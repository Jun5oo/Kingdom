using DG.Tweening;
using System.Collections;
using UnityEngine;

public class TurnEnd : MonoBehaviour
{
    TurnSystem turnSystem;
    PlayerManager playerManager; 

    [SerializeField] Renderer buttonRenderer;

    [SerializeField] Texture2D localTurn;
    [SerializeField] Texture2D remoteTurn; 

    void Start()
    {
        turnSystem = ServiceLocator.Get<TurnSystem>();
        playerManager = ServiceLocator.Get<PlayerManager>();

        this.transform.localScale = Vector3.zero; 

        Unsubscribe();
        Subscribe(); 
    }

    public void StartTurn(int playerID)
    {
        Texture2D texture = null;

        if (playerID == playerManager.Local.PlayerID)
            texture = localTurn; 
        else
            texture = remoteTurn; 

        UpdateButton(texture);
        DoExpand();
    }
    public void EndTurn(int playerID) => DoShrink(); 

    public void UpdateButton(Texture2D texture)
    {
        Material mat = buttonRenderer.sharedMaterial;
        if (mat.HasProperty("_BaseMap"))
            mat.SetTexture("_BaseMap", texture); 
    }

    public void DoExpand() => transform.DOScale(Vector3.one, 0.2f); 
    public void DoShrink() => transform.DOScale(Vector3.zero, 0.2f);

    public void OnMouseDown()
    {
        if (turnSystem.TurnState == TurnState.Unable)
            return; 

        turnSystem.EndTurn(); 
    }

    void Subscribe()
    {
        turnSystem.onTurnStarted += StartTurn;
        turnSystem.onTurnEnded += EndTurn; 
    }

    void Unsubscribe()
    {
        turnSystem.onTurnStarted -= StartTurn;
        turnSystem.onTurnEnded -= EndTurn;
    }
}
