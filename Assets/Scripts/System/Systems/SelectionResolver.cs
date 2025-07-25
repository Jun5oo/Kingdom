using UnityEngine;

public class SelectionResolver {

    TokenManager tokenManager;

    TurnSystem turnSystem;
    ActionSystem actionSystem; 

    public SelectionResolver()
    {
        tokenManager = ServiceLocator.Get<TokenManager>();  

        turnSystem = ServiceLocator.Get<TurnSystem>();
        actionSystem = ServiceLocator.Get<ActionSystem>();
    }

    public ISelectable Resolve(RaycastHit hit)
    {
        if(hit.collider.TryGetComponent<ISelectable>(out ISelectable direct))
            return direct; 

        if(hit.collider.TryGetComponent<GridCell>(out GridCell gridCell))
        {
            Vector2Int gridPos = gridCell.GetGridPosition(); 

            if(tokenManager.TryGetTokenFrom(gridPos, out Token token))
            {
                if(token.TryGetComponent<ISelectable>(out ISelectable indirect))
                    return indirect; 
            }
        }

        return null; 
    }

    public bool IsValid(ISelectable selectable)
    {
        if (selectable == null)
            return false;

        /*
        if (turnSystem.GetCurrentTurnPlayerID() != selectable.Entity.OwnerID)
            return false;
        */ 
        /*
        if (turnSystem.TurnState != TurnState.PlayerTurn)
            return false;
        */
        if (actionSystem.IsActionInProgress())
            return false;

        if (!selectable.IsSelectable())
            return false;

        return true;
    }
}
