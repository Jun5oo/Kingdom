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

    // 게임 시작을 일정 시간 뒤에 실행 (ex. 오프닝 UI 이후)
    void StartGameDelayed()
    {
        gameFlowManager.StartGame();
    }
}
