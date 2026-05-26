using UnityEngine;

/// <summary>
/// 게임 흐름 상태 전환을 담당하는 상태 머신.
/// firstID/secondID(선·후공), firstCard/secondCard(왕 카드)를 상태 간에 공유하는 컨텍스트로 활용한다.
/// </summary>
public class GameFlowStateMachine
{
    public int firstID;   // 선공 플레이어 ID
    public int secondID;  // 후공 플레이어 ID

    public Card firstCard;  // 선공 플레이어의 왕 카드
    public Card secondCard; // 후공 플레이어의 왕 카드

    IGameState currentState;

    /// <summary> 새 상태로 전환하고 Enter()를 실행한다. </summary>
    public void Enter(IGameState state)
    {
        currentState = state;
        state.Enter();
    }
}
