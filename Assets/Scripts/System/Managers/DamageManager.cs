using Cysharp.Threading.Tasks;
using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class DamageManager
{
    // 데미지 뿐만 아니라, 한 전투에서의 과정(유닛 처치, 사망 등)을 관리하므로 CombatManager로 클래스 이름을 변경해줄 예정 

    PlayerManager playerManager; 
    TokenManager tokenManager;
    DamageDisplayer displayer;

    public Action<int> OnKingDefeated;

    // 플레이어의 유닛이 사망했을 때, 사전 준비작업을 하기 위한 이벤트 
    public Action<int, Token> OnPrepareUnitDeath; 
    // 플레이어의 유닛이 사망했을 때 
    public Action<int, Token> OnPlayerUnitDead;
    // 플레이어의 유닛이 유닛을 처치했을 때  
    public Action<Token, Token> OnPlayerUnitKilledEnemy;

    public void Init()
    {
        this.playerManager = ServiceLocator.Get<PlayerManager>();
        this.tokenManager = ServiceLocator.Get<TokenManager>();
        this.displayer = new DamageDisplayer();
    }

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
    public void ProcessIndirectDamage(Token token, int damage)
    {
        if (token.Tag == UnitTag.King)
            return; 

        if(tokenManager.TryGetKingTokenFrom(token.OwnerID, out Token king))
        {
            if(king.TryGetComponent<IDamageable>(out IDamageable damageable))
            {
                damage = damageable.TakeDamage(damage, false);
                displayer.Display(damage, king); 
            }
        }
    }
    public int ProcessCounterDamage(Token defender, Token attacker, int defenderCP)
    {
        if (!attacker.TryGetComponent<IDamageable>(out IDamageable damageable))
        {
            Debug.Log("공격한 유닛은 IDamageable이 아닙니다.");
            return 0; 
        }

        if (defender.AttackRange == null || defender.AttackRange.Count == 0 )
        {
            Debug.Log("이 유닛은 반격이 불가능합니다.");
            return 0; 
        }

        Vector2Int defenderPos = tokenManager.GetGridPositionOfToken(defender);
        Vector2Int attackerPos = tokenManager.GetGridPositionOfToken((attacker));

        // 반격 가능 여부 확인
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

        // 반격 데미지 적용
        int damage = damageable.TakeDamage(defenderCP, false);
        displayer.Display(damage, attacker);

        return damage; 
    }
    public void IsKingDefeated()
    {
        tokenManager.TryGetKingTokenFrom(playerManager.Local.PlayerID, out Token local); 
        tokenManager.TryGetKingTokenFrom(playerManager.Remote.PlayerID, out Token remote);

        if (local == null)
        {
            Debug.LogError("로컬 플레이어의 왕을 찾을 수 없습니다.");
            return; 
        }

        if(remote == null)
        {
            Debug.LogError("상대 플레이어의 왕을 찾을 수 없습니다.");
            return; 
        }

        if(local.IsDead && remote.IsDead)
        {
            // 무승부 
            OnKingDefeated?.Invoke(-1);
            return;
        }

        if (local.IsDead)
            OnKingDefeated?.Invoke(local.OwnerID);
        else if (remote.IsDead)
            OnKingDefeated?.Invoke(remote.OwnerID);
        else
            return; 
    }

    public void ProcessUnitDeath(Token killer, Token victim)
    {
        if(killer.OwnerID == victim.OwnerID)
        {
            Debug.Log("현재 시스템에서는 같은 팀을 처치할 수 없습니다. 뭔가 문제가 발생했습니다.");
            return; 
        }

        OnPrepareUnitDeath?.Invoke(victim.OwnerID, victim);

        Vector2Int killerPosition = tokenManager.GetGridPositionOfToken(killer);
        Vector2Int victimPosition = tokenManager.GetGridPositionOfToken(victim);

        EventBus<UnitDeadEvent>.Publish(new UnitDeadEvent { killer = killer, victim = victim , victimPosition = victimPosition, killerPosition = killerPosition }); 
        
        OnPlayerUnitKilledEnemy?.Invoke(killer, victim);
        OnPlayerUnitDead?.Invoke(victim.OwnerID, victim);

        tokenManager.DestroyToken(victim);
    }
}
