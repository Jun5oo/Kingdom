using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Token : BaseObject, IDamageable, IDestructible, IBuffable 
{
    [Header("RunTime Data")]
    [SerializeField] int currentLevel; 
    [SerializeField] int currentCP;
    [SerializeField] int ownerID;

    [SerializeField] TokenMovement movement;
    [SerializeField] TokenInteraction interaction;
    [SerializeField] TokenView view;

    List<IBuff> buffs;
    List<IPassive> passives;

    bool isDead;

    public UnitCardData UnitData { get { return Data as UnitCardData; } }
    
    public int CP { get { return currentCP; } }
    public int MAXCP { get { return UnitData.GetCP(currentLevel); } }
    public int Movement { get { return UnitData.GetMovement(currentLevel); } }
    public int Level { get { return currentLevel; } }
    public override int OwnerID { get { return ownerID; } }

    public UnitTag Tag { get { return UnitData.Tag; } }

    public List<Vector2Int> MoveableRange { get { return UnitData.MoveRange; } }
    public List<Vector2Int> AttackRange { get { return UnitData.AttackRange; } }


    // 생성한 주체, ex) 무덤을 생성한 것은 언데드 왕
    [SerializeField] CardData sourceObject; 
    // 생성에 필요한 재료, ex) 무덤의 original cardData 또는 업그레이드에 사용된 오브젝트 
    [SerializeField] List<UnitCardData> sourceObjects;
    public CardData SourceObject { get { return sourceObject; } }
    public List<UnitCardData> SourceObjects { get { return  sourceObjects; } }

    public void Init(UnitCardData unitData, int playerID, CardData sourceObject = null, List<UnitCardData> sourceObjects = null, int spawnLevel = 1)
    {
        base.Init(unitData);

        this.currentLevel = spawnLevel; 
        this.currentCP = MAXCP;
        this.ownerID = playerID;

        movement.Init();
        interaction.Init(this); 

        buffs = new List<IBuff>();
        passives = new List<IPassive>();

        PassiveFactory passiveFactory = ServiceLocator.Get<PassiveFactory>(); 

        foreach(var passive in UnitData.Passive)
        {
            IPassive created = passiveFactory.CreatePassive(passive, this);
            passives.Add(created);
            created.Deactivate(); 
            created.Activate(); 
        }

        isDead = false;

        this.sourceObject = sourceObject;
        
        if (sourceObjects != null)
            this.sourceObjects = sourceObjects;
        else
            this.sourceObjects = new List<UnitCardData>(); 
    }

    public Action<int> OnCPUpdate;

    #region IDamageable 
    public bool IsAllies(int playerID) => OwnerID == playerID;
    public int TakeDamage(int damage, bool isDirect = false)
    {
        if (isDirect && Tag == UnitTag.King)
            damage *= 2;

        List<IBuff> removeList = new List<IBuff>(); 

        foreach(var buff in buffs)
        {
            if (buff is IDamageModifierBuff dmgModifier && !buff.IsExpired()) 
                damage = dmgModifier.ModifyDamage(damage);

            if (buff.IsExpired())
                removeList.Add(buff); 
        }

        foreach(var buff in removeList)
            RemoveBuff(buff); 

        currentCP -= damage;
        OnCPUpdate?.Invoke(currentCP);

        if (currentCP <= 0)
            isDead = true; 

        view.OnUpdateCP(currentCP);

        return damage; 
    }
    #endregion

    #region IDestructible 
    public void Die()
    {
        Debug.Log($"{this} dead"); 
    }
    public bool IsDead => isDead; 
    #endregion

    #region IBuffable
    public void AddBuff(IBuff buff)
    {
        if (!buff.IsStackable() && buffs.Contains(buff))
            return; 
        buffs.Add(buff); 
    }
    public void RemoveBuff(IBuff buff) => buffs.Remove(buff);
    public bool CanApply(IBuff buff)
    {
        if (!buff.IsStackable() && buffs.Contains(buff))
            return false;
        return true; 
    }

    #endregion

    public T GetSourceObjects<T>() where T : BaseObject
    {
        return SourceObject as T; 
    }

    public List<T> GetSourceTokens<T>() where T: BaseObject
    {
        return SourceObjects as List<T>; 
    }

    void OnDestroy()
    {
        foreach(var buff in buffs)
            RemoveBuff(buff); 

        buffs.Clear(); 

        foreach(var passive in passives)
            passive.Deactivate();

        passives.Clear();
        sourceObjects.Clear(); 

        OnCPUpdate = null;
    }
}
