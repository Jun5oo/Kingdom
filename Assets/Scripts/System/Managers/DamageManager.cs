using System;
using UnityEngine;

public class DamageManager
{
    PlayerManager playerManager; 
    TokenManager tokenManager;
    UIManager uiManager;

    public Action<int> OnKingDefeated; 
     
    public void Init(PlayerManager playerManager, TokenManager tokenManager, UIManager uiManager)
    {
        this.playerManager = playerManager;
        this.tokenManager = tokenManager;
        this.uiManager = uiManager; 
    }

    public void ProcessCombat(Token attacker, Token defender)
    {
        int attackerCP = attacker.CP;

        if (defender.TryGetComponent<IDamageable>(out IDamageable damageable))
        {
            damageable.TakeDamage(attackerCP);
            uiManager.ShowDamagePopup(attackerCP, defender.transform.position);
        }

        if (!defender.IsKing)
            ProcessKingDamage(defender, attackerCP);
    }
    public void ProcessKingDamage(Token token, int damage)
    {
        if(tokenManager.TryGetKingTokenFrom(token.OwnerPlayerID, out Token king)){
            
            if(king.TryGetComponent<IDamageable>(out IDamageable damageable))
            {
                damageable.TakeDamage(damage, false);
                uiManager.ShowDamagePopup(damage, king.transform.position); 
            }
        }
    }
    public void TryProcessCounterAttack(Token defender, Token attacker, int cp)
    {
        if (defender == null)
        {
            Debug.Log("Defender를 찾을 수 없습니다."); 
            return;
        }

        if (defender.CurrentAttackRange == null || defender.CurrentAttackRange.Count == 0)
        {
            Debug.Log("이 유닛은 반격이 불가능합니다.");
            return;
        }

        Vector2Int defenderPos = tokenManager.GetGridPositionOfToken(defender);
        Vector2Int attackerPos = tokenManager.GetGridPositionOfToken((attacker));

        if (!attacker.TryGetComponent<IDamageable>(out IDamageable damageable))
        {
            Debug.Log("Attacker는 IDamageable이 아닙니다."); 
            return;
        }

        foreach (var position in defender.CurrentAttackRange)
        {
            if (defenderPos + position == attackerPos)
            {
                damageable.TakeDamage(cp, false);
                uiManager.ShowDamagePopup(cp, attacker.transform.position);

                if (!attacker.IsKing)
                    ProcessKingDamage(attacker, cp); 

                return; 
            }
        }
    }
    public void CheckForKingDefeat()
    {
        tokenManager.TryGetKingTokenFrom(playerManager.LocalPlayerData.PlayerID, out Token local); 
        tokenManager.TryGetKingTokenFrom(playerManager.RemotePlayerData.PlayerID, out Token remote);

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

        if(local.CP <= 0 && remote.CP <= 0)
        {
            OnKingDefeated?.Invoke(-1);
            return; 
        }

        if (local.CP <= 0)
            OnKingDefeated?.Invoke(local.OwnerPlayerID);
        else if (remote.CP <= 0)
            OnKingDefeated?.Invoke(remote.OwnerPlayerID);
        else
            return; 
    }
    public void TryDestroyToken(Token token)
    {
        if (token.CP > 0)
            return;

        IDeathBehaviour deathBehaviour = token.DeathBehaviour;
        deathBehaviour.OnDeath(token, tokenManager); 
    }
}
