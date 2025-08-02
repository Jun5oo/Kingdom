using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

public class DamageManager
{
    PlayerManager playerManager; 
    TokenManager tokenManager;
    DamageDisplayer displayer;

    public Action<int> OnKingDefeated;

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
    public void ProcessKingDamage(Token token, int damage)
    {
        if (token.IsKing)
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

        if (defender.AttackRange == null || defender.AttackRange.Count == 0)
        {
            Debug.Log("이 유닛은 반격이 불가능합니다.");
            return 0; 
        }

        Vector2Int defenderPos = tokenManager.GetGridPositionOfToken(defender);
        Vector2Int attackerPos = tokenManager.GetGridPositionOfToken((attacker));


        foreach (var position in defender.AttackRange)
        {
            if (defenderPos + position == attackerPos)
            {
                int damage = damageable.TakeDamage(defenderCP, false);
                displayer.Display(damage, attacker);

                return damage; 
            }
        }

        return 0; 
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

    public async UniTask ProcessUnitDeath(Token killer, Token victim)
    {
        if(killer.OwnerID == victim.OwnerID)
        {
            Debug.Log("현재 시스템에서는 같은 팀을 처치할 수 없습니다. 뭔가 문제가 발생했습니다.");
            return; 
        }

        OnPlayerUnitKilledEnemy?.Invoke(killer, victim);
        OnPlayerUnitDead?.Invoke(victim.OwnerID, victim);

        tokenManager.DestroyToken(victim);
    }
}
