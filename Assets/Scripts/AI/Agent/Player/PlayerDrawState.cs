using System.Collections;
using UnityEngine;

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
