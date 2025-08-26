using UnityEngine;

public class DestroySelf : IPassive
{
    TurnSystem turnSystem;
    BaseObject owner; 
    
    int duration; 

    public DestroySelf(BaseObject owner)
    {
        this.turnSystem = ServiceLocator.Get<TurnSystem>();
        this.owner = owner;
        
        duration = 2;
    }

    public void Activate()
    {
        turnSystem.onTurnStarted += ReduceDuration; 
    }

    public void Deactivate()
    {
        turnSystem.onTurnStarted -= ReduceDuration;
    }

    void ReduceDuration(int playerID)
    {
        if(playerID == owner.OwnerID)
            duration -= 1; 

        if(duration <= 0)
        {
            TokenManager tokenManager = ServiceLocator.Get<TokenManager>();
            tokenManager.DestroyToken(owner as Token); 
        }
    }
}
