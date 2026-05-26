using UnityEngine;

/// <summary>
/// AI 봇의 AgentController 구현체.
/// 현재 페이즈 상태는 미구현이며, GetDesiredLevel()로 턴 수에 따른 목표 레벨을 제공한다.
/// </summary>
public class BotController : AgentController
{
    public override void InitializePhaseStates()
    {
    }

    /// <summary>
    /// 턴 수 기준으로 목표 레벨을 반환한다.
    /// 0~2턴: 레벨 1, 3~5턴: 레벨 2, 6턴 이후: 레벨 3 (최대).
    /// </summary>
    public int GetDesiredLevel(int turnCount)
    {
        return Mathf.Min(turnCount / 3 + 1, 3);
    }
}
