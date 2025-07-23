using System;
using System.Collections;
using UnityEngine;

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
