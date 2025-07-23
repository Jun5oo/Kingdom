using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class PlayerSupportState : IPhaseState
{
    private AgentController agent;
    private bool hasMovedToNextPhase = false;

    public void Enter(AgentController agent)
    {
        this.agent = agent;
        hasMovedToNextPhase = false;
        SetSelectableCardsInHand();
        RegisterBatchingEvent();
    }

    private void SetSelectableCardsInHand()
    {
    }

    private void RegisterBatchingEvent()
    {
    }


    private void OnCardBatching()
    {
        if (hasMovedToNextPhase) return;
        hasMovedToNextPhase = true;

        GoToNextPhase();
    }

    private void GoToNextPhase()
    {
    }

    public void Execute() { }

    public void Exit()
    {
        UnregisterBatchingEvent();
    }

    private void UnregisterBatchingEvent()
    {
    }
}
