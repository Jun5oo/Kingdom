using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 게임 종료 상태.
/// HoverSystem/SelectionSystem/TurnSystem을 비활성화하고 5초 후 타이틀 씬으로 전환한다.
/// </summary>
public class GameOver : IGameState
{
    /// <summary>
    /// 입력 시스템을 모두 비활성화하고, 5초(언스케일드 시간) 후 "Title" 씬을 로드한다.
    /// </summary>
    public async UniTask Enter()
    {
        Debug.Log("GameOver");

        HoverSystem hoverSystem = ServiceLocator.Get<HoverSystem>();
        SelectionSystem selectionSystem = ServiceLocator.Get<SelectionSystem>();
        TurnSystem turnSystem = ServiceLocator.Get<TurnSystem>();

        hoverSystem.DisableSystem();
        selectionSystem.DisableSystem();
        turnSystem.DisableSystem();

        // 결과 화면을 5초간 유지한 뒤 타이틀로 복귀 (UnscaledDeltaTime: 일시정지 상태에도 동작)
        await UniTask.Delay(System.TimeSpan.FromSeconds(5), DelayType.UnscaledDeltaTime, PlayerLoopTiming.Update);

        SceneManager.LoadScene("Title");
    }
}
