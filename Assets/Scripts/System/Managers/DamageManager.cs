using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

/// <summary>
/// 전투 과정(데미지 처리, 유닛 사망, 왕 사망 감지)을 관리하는 매니저.
/// (추후 CombatManager로 이름 변경 예정)
/// </summary>
public class DamageManager
{
    PlayerManager playerManager;
    TokenManager tokenManager;
    DamageDisplayer displayer;

    public Action<int> OnKingDefeated;              // 왕이 사망했을 때 loserID와 함께 발생
    public Action<int, Token> OnPrepareUnitDeath;   // 유닛 사망 전 사전 처리 이벤트 (패시브 대응용)
    public Action<int, Token> OnPlayerUnitDead;     // 유닛이 사망했을 때 발생 (SummonGraveyard 등 패시브 트리거)
    public Action<Token, Token> OnPlayerUnitKilledEnemy; // 유닛이 적을 처치했을 때 발생 (GainAbilityCoin 등 패시브 트리거)

    public void Init()
    {
        this.playerManager = ServiceLocator.Get<PlayerManager>();
        this.tokenManager = ServiceLocator.Get<TokenManager>();
        this.displayer = new DamageDisplayer();
    }

    /// <summary>
    /// 공격자의 CP를 방어자에게 직접 데미지로 적용한다.
    /// 실제 적용된 데미지(버프 적용 후)를 반환하며 팝업을 표시한다.
    /// </summary>
    public int ProcessDamage(Token attacker, Token defender)
    {
        int damage = attacker.CP;

        if (defender.TryGetComponent<IDamageable>(out IDamageable damageable))
        {
            damage = damageable.TakeDamage(damage, true);
            displayer.Display(damage, defender);
        }

        return damage;
    }

    /// <summary>
    /// 유닛이 입은 데미지를 해당 유닛 소유자의 왕에게 간접 데미지로 전달한다.
    /// 왕 유닛이나 무덤 유닛은 간접 데미지를 전달하지 않는다.
    /// </summary>
    public void ProcessIndirectDamage(Token token, int damage)
    {
        if (token.Tag == UnitTag.King)
            return;

        if (token.Tag == UnitTag.Graveyard)
            return;

        if (tokenManager.TryGetKingTokenFrom(token.OwnerID, out Token king))
        {
            if (king.TryGetComponent<IDamageable>(out IDamageable damageable))
            {
                damage = damageable.TakeDamage(damage, false);
                displayer.Display(damage, king);
            }
        }
    }

    /// <summary>
    /// 반격 데미지를 공격자에게 적용한다.
    /// 방어자가 공격 범위 내에 공격자가 없으면 반격 불가로 0을 반환한다.
    /// </summary>
    public int ProcessCounterDamage(Token defender, Token attacker, int defenderCP)
    {
        if (!attacker.TryGetComponent<IDamageable>(out IDamageable damageable))
        {
            Debug.Log("공격한 유닛은 IDamageable이 아닙니다.");
            return 0;
        }

        if (defender.AttackRange == null || defender.AttackRange.Count == 0)
        {
            Debug.Log("이 유닛은 반격이 불가능합니다.");
            return 0;
        }

        Vector2Int defenderPos = tokenManager.GetGridPositionOfToken(defender);
        Vector2Int attackerPos = tokenManager.GetGridPositionOfToken(attacker);

        bool canCounter = false;
        foreach (var position in defender.AttackRange)
        {
            if (defenderPos + position == attackerPos)
            {
                canCounter = true;
                break;
            }
        }

        if (!canCounter)
        {
            Debug.Log("반격 불가능: 공격 범위 밖");
            return 0;
        }

        int damage = damageable.TakeDamage(defenderCP, false);
        displayer.Display(damage, attacker);

        return damage;
    }

    /// <summary>
    /// 양쪽 왕의 사망 여부를 확인하고 게임 종료 이벤트를 발생시킨다.
    /// 양쪽 모두 사망이면 무승부(-1)를 전달한다.
    /// </summary>
    public void IsKingDefeated()
    {
        tokenManager.TryGetKingTokenFrom(playerManager.Local.PlayerID, out Token local);
        tokenManager.TryGetKingTokenFrom(playerManager.Remote.PlayerID, out Token remote);

        if (local == null)
        {
            Debug.LogError("로컬 플레이어의 왕을 찾을 수 없습니다.");
            return;
        }

        if (remote == null)
        {
            Debug.LogError("상대 플레이어의 왕을 찾을 수 없습니다.");
            return;
        }

        if (local.IsDead && remote.IsDead)
        {
            OnKingDefeated?.Invoke(-1); // 무승부
            return;
        }

        if (local.IsDead)
            OnKingDefeated?.Invoke(local.OwnerID);
        else if (remote.IsDead)
            OnKingDefeated?.Invoke(remote.OwnerID);
    }

    /// <summary>
    /// 유닛 사망을 처리한다. 패시브 이벤트를 발생시키고 토큰을 파괴한다.
    /// 같은 팀끼리는 처치할 수 없다.
    /// </summary>
    public void ProcessUnitDeath(Token killer, Token victim)
    {
        if (killer.OwnerID == victim.OwnerID)
        {
            Debug.Log("현재 시스템에서는 같은 팀을 처치할 수 없습니다. 뭔가 문제가 발생했습니다.");
            return;
        }

        OnPrepareUnitDeath?.Invoke(victim.OwnerID, victim);

        OnPlayerUnitKilledEnemy?.Invoke(killer, victim);
        OnPlayerUnitDead?.Invoke(victim.OwnerID, victim);

        tokenManager.DestroyToken(victim);
    }
}
