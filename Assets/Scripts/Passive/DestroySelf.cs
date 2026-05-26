using UnityEngine;

/// <summary>
/// 소유자 플레이어 턴이 시작될 때마다 duration을 1씩 감소시키고, 0이 되면 스스로를 파괴하는 패시브.
/// 무덤 토큰처럼 수명이 제한된 유닛에 적용된다.
/// </summary>
public class DestroySelf : IPassive
{
    TurnSystem turnSystem;
    BaseObject owner;

    int duration; // 남은 수명 (소유자 턴 기준)

    public DestroySelf(BaseObject owner)
    {
        this.turnSystem = ServiceLocator.Get<TurnSystem>();
        this.owner = owner;

        duration = 2;
    }

    public void Activate()
    {
        turnSystem.onTurnStarted += ReduceDuration;
    }

    public void Deactivate()
    {
        turnSystem.onTurnStarted -= ReduceDuration;
    }

    /// <summary>
    /// 소유자 플레이어 턴 시작 시 duration을 1 감소한다.
    /// duration이 0 이하이면 TokenManager를 통해 자신을 파괴한다.
    /// </summary>
    void ReduceDuration(int playerID)
    {
        if (playerID == owner.OwnerID)
            duration -= 1;

        if (duration <= 0)
        {
            TokenManager tokenManager = ServiceLocator.Get<TokenManager>();
            tokenManager.DestroyToken(owner as Token);
        }
    }
}
