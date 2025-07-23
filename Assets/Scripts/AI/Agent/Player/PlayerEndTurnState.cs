using System.Collections;
using UnityEngine;

public class PlayerEndTurnState : IPhaseState
{
    private AgentController agent;

    public void Enter(AgentController agent)
    {
        this.agent = agent;
        // InGameManager.Instance.AdvancePhase();
    }

    public void Execute()
    {
    }

    public void Exit() 
    {
    }
}