using UnityEngine;

public class SelectionResolver {

    TokenManager tokenManager;
    ActionSystem actionSystem;

    public SelectionResolver()
    {
        tokenManager = ServiceLocator.Get<TokenManager>();  
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
        {
            Debug.Log($"{selectable}은 null 입니다.");
            return false;
        }

        if (actionSystem.IsActionInProgress())
        {
            Debug.Log("현재 Action이 InProgress 상태입니다."); 
            return false;
        }

        if (!selectable.IsSelectable())
        {
            Debug.Log($"{selectable}이 InSelectable 상태입니다."); 
            return false;
        }

        if (selectable.BaseObject == null)
        {
            Debug.Log($"{selectable}의 BaseObject가 존재하지 않습니다."); 
            return false;
        }

        return true;
    }
}
