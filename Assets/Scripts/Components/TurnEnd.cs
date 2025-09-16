using DG.Tweening;
using UnityEngine;
using UnityEngine.UIElements;

public class TurnEnd : MonoBehaviour
{
    TurnSystem turnSystem;
    ActionSystem actionSystem; 

    [SerializeField] Renderer buttonRenderer;

    [SerializeField] Texture2D myTurn;
    [SerializeField] Texture2D opponentTurn;

    void OnDisable()
    {
        Unsubscribe(); 
    }

    void Start()
    {
        turnSystem = ServiceLocator.Get<TurnSystem>();
        actionSystem = ServiceLocator.Get<ActionSystem>();

        this.transform.localScale = Vector3.zero; 

        Unsubscribe();
        Subscribe(); 
    }

    public void MyTurn()
    {
        OnUpdateTexture(myTurn);
        DoExpand(); 
    }
    public void OpponentTurn()
    {
        OnUpdateTexture(opponentTurn);
        DoExpand();
    }
    public void EndTurn()
    {
        DoShrink(); 
    }

    public void OnUpdateTexture(Texture2D texture)
    {
        Material mat = buttonRenderer.sharedMaterial;
        if (mat.HasProperty("_BaseMap"))
            mat.SetTexture("_BaseMap", texture); 
    }
    public void DoExpand()
    {
        transform.DOScale(Vector3.one, 0.2f); 
    }
    public void DoShrink()
    {
        transform.DOScale(Vector3.zero, 0.2f); 
    }

    public void OnMouseDown()
    {
        if (turnSystem.TurnState == TurnState.Unable)
            return;

        if (actionSystem.IsActionInProgress())
            return;

        if (!turnSystem.IsMyTurn())
            return; 

        turnSystem.EndTurn(); 
    }

    void Subscribe()
    {
        turnSystem.OnPlayerTurnStarted += MyTurn;
        turnSystem.OnOpponentTurnStarted += OpponentTurn; 
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
