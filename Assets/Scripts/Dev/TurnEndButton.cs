using UnityEngine;

public class TurnEndButton : MonoBehaviour
{
    TurnSystem turnSystem; 

    [SerializeField] Renderer buttonRenderer;

    [SerializeField] Material turnOnMaterial; 
    Material turnOffMaterial;

    void Start()
    {
        turnSystem = ServiceLocator.Get<TurnSystem>();
        turnOffMaterial = buttonRenderer.sharedMaterial;

        turnSystem.OnTurnStarted -= TurnOn;
        turnSystem.OnTurnEnded -= TurnOff;
        turnSystem.OnTurnStarted += TurnOn;
        turnSystem.OnTurnEnded += TurnOff; 
    }

    public void TurnOn()
    {
        if(turnSystem.IsMyTurn())
            buttonRenderer.sharedMaterial = turnOnMaterial; 
    }

    public void TurnOff()
    {
        buttonRenderer.material = turnOffMaterial; 
    }

    public void OnMouseDown()
    {
        if (turnSystem.TurnState == TurnState.Unable)
            return; 
        turnSystem.EndTurn(); 
    }
}
