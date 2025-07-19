using UnityEngine;

public class TurnEndButton : MonoBehaviour
{
    TurnManager turnManager; 

    [SerializeField] Renderer renderer;

    [SerializeField] Material turnOnMaterial; 
    Material turnOffMaterial;

    void Start()
    {
        turnManager = ServiceLocator.Get<TurnManager>();
        turnOffMaterial = renderer.material;

        turnManager.OnTurnStarted -= TurnOn;
        turnManager.OnTurnEnded -= TurnOff;
        turnManager.OnTurnStarted += TurnOn;
        turnManager.OnTurnEnded += TurnOff; 
    }

    public void TurnOn()
    {
        if(turnManager.IsMyTurn())
            renderer.material = turnOnMaterial; 
    }

    public void TurnOff()
    {
        renderer.material = turnOffMaterial; 
    }

    public void OnMouseDown()
    {
        if (turnManager.TurnState == TurnState.Unable)
            return; 
        turnManager.EndTurn(); 
    }
}
