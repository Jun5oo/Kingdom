using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] CardSystem cardSystem;
    [SerializeField] GridSystem gridSystem;
    [SerializeField] UISystem uiSystem;
    [SerializeField] SelectionSystem selectionSystem;
    [SerializeField] ActionSystem actionSystem;
    [SerializeField] GameFlowManager gameFlowManager;

    void Start()
    {
        // Init 순서 중요!!
        cardSystem.Init(gridSystem, uiSystem, selectionSystem, actionSystem);
        selectionSystem.Init(gridSystem, actionSystem);
        gameFlowManager.Init(cardSystem); // ← GameFlowManager에 cardSystem 주입

        Invoke("StartGameDelayed", 4f);
    }

    void StartGameDelayed()
    {
        gameFlowManager.StartGame();
    }
}
