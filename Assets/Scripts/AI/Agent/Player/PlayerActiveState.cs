using System.Collections;
using UnityEngine;

public class PlayerActiveState : IPhaseState
{
    private AgentController agent;

    public void Enter(AgentController agent)
    {
        this.agent = agent;
        ShowText();
    }

    public void Execute() 
    {
    }

    /// <summary>
    /// 턴 표시
    /// </summary>
    private void ShowText()
    {
        // TODO: 로컬라이징
    }

    /// <summary>
    

    public void Exit() 
    {
    }
}
