using System.Collections;
using UnityEngine;

public class GameOver : IGameState
{
    HoverSystem hoverSystem;
    SelectionSystem selectionSystem;

    public GameOver()
    {
        hoverSystem = ServiceLocator.Get<HoverSystem>(); 
        selectionSystem = ServiceLocator.Get<SelectionSystem>();
    }

    public IEnumerator Enter()
    {
        hoverSystem.DisableSystem(); 
        selectionSystem.DisableSystem();

        yield return null; 
    }
}
