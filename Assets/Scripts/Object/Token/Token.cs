using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 그리드 위에 배치된 유닛 토큰.
/// IDamageable(데미지/버프 처리), IDestructible(사망 판정), IBuffable(버프 부착/제거)을 구현한다.
/// 레벨별 스탯은 CardData 배열에서 읽으며, Passive 목록은 Init() 시 PassiveFactory로 생성된다.
/// </summary>
public class Token : BaseObject, IDamageable, IDestructible, IBuffable
{
    [Header("RunTime Data")]
    [SerializeField] int currentLevel;
    [SerializeField] int currentCP;

    [SerializeField] List<Vector2Int> attackVectors; // 현재 레벨의 공격 범위 벡터 목록
    [SerializeField] List<Vector2Int> moveVectors;   // 현재 레벨의 이동 범위 벡터 목록

    [SerializeField] TokenMovement movement;
    [SerializeField] TokenInteraction interaction;
    [SerializeField] TokenView view;

    [SerializeField] CardData sourceObject;         // 이 토큰을 생성한 주체 (예: 무덤을 소환한 언데드 왕)
    [SerializeField] List<CardData> sourceObjects;  // 생성에 사용된 재료 목록 (예: 업그레이드 소스)

    List<IBuff> buffs;
    List<IPassive> passives;

    bool isDead;

    #region Data Property
    public int CP { get { return currentCP; } }
    public int MAXCP { get { return Data.CP[currentLevel - 1]; } }
    public int Movement { get { return Data.MoveRange[currentLevel - 1]; } }
    public int Level { get { return currentLevel; } }

    public UnitTag Tag { get { return Data.Tag; } }

    public List<Vector2Int> MoveableRange { get { return moveVectors; } }
    public List<Vector2Int> AttackRange { get { return attackVectors; } }

    public CardData SourceObject { get { return sourceObject; } }
    public List<CardData> SourceObjects { get { return sourceObjects; } }

    #endregion


    /// <summary>
    /// 토큰을 초기화한다.
    /// spawnLevel에 맞는 스탯을 설정하고, RangeResolver로 공격·이동 벡터를 계산하며,
    /// CardData.Passive 목록을 PassiveFactory로 생성하여 즉시 활성화한다.
    /// </summary>
    public void Init(CardData unitData, int playerID, CardData sourceObject = null, List<CardData> sourceObjects = null, int spawnLevel = 1)
    {
        base.Init(unitData, playerID);

        this.currentLevel = spawnLevel;
        this.currentCP = MAXCP;

        movement.Init();
        interaction.Init(this);

        RangeResolver resolver = ServiceLocator.Get<RangeResolver>();

        attackVectors = resolver.Resolve(Data.AttackType[Level - 1], Data.AttackRange[Level - 1]);
        moveVectors = resolver.Resolve(Data.MoveType[Level - 1], Data.MoveRange[Level - 1]);

        buffs = new List<IBuff>();
        passives = new List<IPassive>();

        PassiveFactory passiveFactory = ServiceLocator.Get<PassiveFactory>();

        foreach (var passive in unitData.Passive)
        {
            IPassive created = passiveFactory.CreatePassive(passive, this);

            if (created != null)
            {
                passives.Add(created);
                created.Deactivate();
                created.Activate();
            }
        }

        isDead = false;

        this.sourceObject = sourceObject;

        if (sourceObjects != null)
            this.sourceObjects = sourceObjects;
        else
            this.sourceObjects = new List<CardData>();
    }

    public Action<int> OnCPUpdate; // CP가 변경될 때마다 발생 (HUD 갱신용)

    #region IDamageable
    public bool IsAllies(int playerID) => OwnerID == playerID;

    /// <summary>
    /// 데미지를 처리하고 실제로 감소된 CP 양을 반환한다.
    /// isDirect=true이고 King이면 데미지 2배 적용.
    /// IDamageModifierBuff를 가진 버프가 있으면 데미지를 수정한 뒤 만료된 버프를 제거한다.
    /// </summary>
    public int TakeDamage(int damage, bool isDirect = false)
    {
        if (isDirect && Tag == UnitTag.King)
            damage *= 2;

        List<IBuff> removeList = new List<IBuff>();

        foreach (var buff in buffs.ToArray())
        {
            if (buff is IDamageModifierBuff dmgModifier && !buff.IsExpired())
                damage = dmgModifier.ModifyDamage(damage);

            if (buff.IsExpired())
                removeList.Add(buff);
        }

        foreach (var buff in removeList)
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
    /// <summary> 스택 불가 버프가 이미 적용되어 있으면 무시하고, 아니면 추가한다. </summary>
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
    public List<T> GetSourceTokens<T>() where T : BaseObject
    {
        return SourceObjects as List<T>;
    }

    void OnDestroy()
    {
        Clear();
    }

    /// <summary> 파괴 시 버프와 패시브를 전부 비활성화·제거하여 이벤트 누수를 방지한다. </summary>
    void Clear()
    {
        foreach (var buff in buffs)
        {
            if (buff == null)
                continue;

            RemoveBuff(buff);
        }

        buffs.Clear();

        foreach (var passive in passives)
        {
            if (passive == null)
                continue;

            passive.Deactivate();
        }

        buffs.Clear();
        passives.Clear();
        sourceObjects.Clear();

        OnCPUpdate = null;
    }
}
