using UnityEngine;

public class GameFlowStateMachine
{
    public int firstID;
    public int secondID;

    public Card firstCard;
    public Card secondCard; 

    IGameState currentState;

    public void Enter(IGameState state)
    {
        currentState = state;
        state.Enter(); 
    }
}
