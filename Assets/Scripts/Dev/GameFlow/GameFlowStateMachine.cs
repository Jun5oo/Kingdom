using UnityEngine;

public class GameFlowStateMachine
{
    public int firstID;
    public int secondID;

    public Card firstCard;
    public Card secondCard; 

    IGameState currentState;

    MonoBehaviour runner; 

    public GameFlowStateMachine(MonoBehaviour runner)
    {
        this.runner = runner; 
    }

    public void Enter(IGameState state)
    {
        currentState = state; 
        runner.StartCoroutine(state.Enter()); 
    }
}
