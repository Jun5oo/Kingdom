using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : IGameState
{
    public async UniTask Enter()
    {
        Debug.Log("GameOver"); 

        HoverSystem hoverSystem = ServiceLocator.Get<HoverSystem>();
        SelectionSystem selectionSystem = ServiceLocator.Get<SelectionSystem>();

        hoverSystem.DisableSystem(); 
        selectionSystem.DisableSystem();

        await UniTask.Delay(System.TimeSpan.FromSeconds(5), DelayType.UnscaledDeltaTime, PlayerLoopTiming.Update);

        SceneManager.LoadScene("Title");
    }
}
