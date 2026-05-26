using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> 봇 서포트 페이즈 상태 (미구현 스텁). </summary>
public class BotSupportState : IPhaseState
{
    private AgentController agent;

    public void Enter(AgentController agent)
    {
        this.agent = agent;
    }

    public void Execute() { }

    public void Exit() { }

}
