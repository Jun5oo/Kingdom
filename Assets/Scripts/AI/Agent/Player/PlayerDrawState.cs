using System.Collections;
using UnityEngine;

/// <summary> 플레이어 드로우 페이즈 상태 (미구현 스텁). </summary>
public class PlayerDrawState : IPhaseState
{
    private AgentController agent;

    public void Enter(AgentController agent)
    {
        this.agent = agent;

        // InGameUIManager.Instance.NextPhaseButton.ChangeButtonActiveState(false, Phase.DrawPhase, 0f);

            Execute();

    }

    public void Execute()
    {
    }

    public void Exit()
    {
    }
}
