using System.Collections.Generic;
using UnityEngine;

public class PlayerMainState : IPhaseState
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
