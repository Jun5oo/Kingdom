using Cysharp.Threading.Tasks;

public class GameOver : IGameState
{
    public UniTask Enter()
    {
        HoverSystem hoverSystem = ServiceLocator.Get<HoverSystem>();
        SelectionSystem selectionSystem = ServiceLocator.Get<SelectionSystem>();

        hoverSystem.DisableSystem(); 
        selectionSystem.DisableSystem();

        return UniTask.CompletedTask; 
    }
}
