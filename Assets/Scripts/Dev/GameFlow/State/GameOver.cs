using Cysharp.Threading.Tasks;
using UnityEngine; 

public class GameOver : IGameState
{
    public UniTask Enter()
    {
        Debug.Log("GameOver"); 

        HoverSystem hoverSystem = ServiceLocator.Get<HoverSystem>();
        SelectionSystem selectionSystem = ServiceLocator.Get<SelectionSystem>();

        hoverSystem.DisableSystem(); 
        selectionSystem.DisableSystem();

        return UniTask.CompletedTask; 
    }
}
