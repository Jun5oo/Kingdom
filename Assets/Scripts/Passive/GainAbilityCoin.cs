using UnityEngine;

/// <summary>
/// 아군 유닛이 적을 처치할 때 소유자에게 어빌리티 코인 1개를 지급하는 패시브.
/// DamageManager.OnPlayerUnitKilledEnemy 이벤트를 구독한다.
/// </summary>
public class GainAbilityCoin : IPassive
{
    BaseObject baseObject;
    DamageManager damageManager;

    public GainAbilityCoin(BaseObject baseObject)
    {
        damageManager = ServiceLocator.Get<DamageManager>();
        this.baseObject = baseObject;
    }

    public void Activate() => damageManager.OnPlayerUnitKilledEnemy += GetAbilityCoin;
    public void Deactivate() => damageManager.OnPlayerUnitKilledEnemy -= GetAbilityCoin;

    /// <summary>
    /// 처치한 유닛의 소유자가 이 패시브 소유자와 같을 때만 어빌리티 코인을 1 추가한다.
    /// </summary>
    void GetAbilityCoin(Token attacker, Token defender)
    {
        if (baseObject.OwnerID != attacker.OwnerID)
            return;

        var abilityResourceSystem = ServiceLocator.Get<AbilityResourceSystem>();
        abilityResourceSystem.Add(attacker.OwnerID, 1);
    }
}
