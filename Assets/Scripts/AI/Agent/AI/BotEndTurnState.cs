using System.Collections;
using UnityEngine;

public class BotEndTurnState : IPhaseState
{
    private AgentController agent;

    public void Enter(AgentController agent)
    {
        this.agent = agent;
    }

    public void Execute()
    {
    }
    public void Exit() 
    {
    }
}