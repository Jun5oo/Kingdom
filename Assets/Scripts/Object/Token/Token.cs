using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Token : BaseObject, IDamageable, IDestructible, IBuffable 
{
    [Header("RunTime Data")]
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
    public int MAXCP { get { return UnitData.CP; } }
    public int Movement { get { return UnitData.Movement; } }
    public bool IsKing { get { return UnitData.IsKing; } }
    public override int OwnerID { get { return ownerID; } }

    public List<Vector2Int> MoveableRange { get { return UnitData.MoveRange; } }
    public List<Vector2Int> AttackRange { get { return UnitData.AttackRange; } }

    public void Init(UnitCardData unitData, int playerID)
    {
        base.Init(unitData); 

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
    }

    public Action<int> OnCPUpdate;

    #region IDamageable 
    public bool IsAllies(int playerID) => OwnerID == playerID;
    public int TakeDamage(int damage, bool isDirect = false)
    {
        if (isDirect && IsKing)
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

    void OnDestroy()
    {
        foreach(var buff in buffs)
            RemoveBuff(buff); 

        buffs.Clear(); 

        foreach(var passive in passives)
            passive.Deactivate(); 

        OnCPUpdate = null;
    }
}
