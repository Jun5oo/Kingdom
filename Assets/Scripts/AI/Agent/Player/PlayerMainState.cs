using System.Collections.Generic;
using UnityEngine;

/// <summary> 플레이어 메인 페이즈 상태 (미구현 스텁). </summary>
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
