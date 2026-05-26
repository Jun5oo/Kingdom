using System;
using System.Collections;
using UnityEngine;

/// <summary> 봇 액티브(행동) 페이즈 상태 (미구현 스텁). </summary>
public class BotActiveState : IPhaseState
{
    private AgentController agent;

    public void Enter(AgentController agent)
    {
        this.agent = agent;

        ShowText();
    }


    private void HandleActivePhase(bool isPlayerTurn)
    {
    }

    private void ShowText()
    {
    }

    public void Execute() 
    {
        
    }

    public void Exit() 
    {
    }
}
