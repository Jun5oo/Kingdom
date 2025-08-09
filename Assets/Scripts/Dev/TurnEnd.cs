using UnityEngine;

public class TurnEnd : MonoBehaviour
{
    TurnSystem turnSystem; 

    [SerializeField] Renderer buttonRenderer;

    [SerializeField] Material green;
    [SerializeField] Material red; 

    Material gray;

    void Start()
    {
        turnSystem = ServiceLocator.Get<TurnSystem>();
        gray = buttonRenderer.sharedMaterial;

        Unsubscribe();
        Subscribe(); 
    }

    public void MyTurn()
    {
        buttonRenderer.sharedMaterial = green; 
    }

    public void OpponentTurn()
    {
        buttonRenderer.sharedMaterial = red;
    }

    public void EndTurn()
    {
        buttonRenderer.material = gray; 
    }

    public void OnMouseDown()
    {
        if (turnSystem.TurnState == TurnState.Unable)
            return; 

        turnSystem.EndTurn(); 
    }

    void Subscribe()
    {
        turnSystem.OnPlayerTurnStarted += MyTurn;
        turnSystem.OnOpponentTurnStarted += MyTurn;
        turnSystem.OnPlayerTurnEnded += EndTurn;
        turnSystem.OnOpponentTurnEnded += EndTurn;
    }

    void Unsubscribe()
    {
        turnSystem.OnPlayerTurnStarted -= MyTurn;
        turnSystem.OnOpponentTurnStarted -= OpponentTurn;
        turnSystem.OnPlayerTurnEnded -= EndTurn;
        turnSystem.OnOpponentTurnEnded -= EndTurn;
    }
}
