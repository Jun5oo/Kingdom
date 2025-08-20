using System;
using UnityEngine;
using UnityEngine.EventSystems; 

public class HoverSystem : MonoBehaviour, IGameSystem
{
    private IHoverable currentHoverable = null;

    GridManager gridManager;
    TokenManager tokenManager;
    ActionSystem actionSystem;

    float hoverPreviewDelay = 0.5f; 
    float hoverStartTime;

    bool isPreviewHovered; 

    public Action<BaseObject> onHoverStart;
    public Action onHoverExit; 

    public void Init()
    {
        this.gridManager = ServiceLocator.Get<GridManager>();
        this.tokenManager = ServiceLocator.Get<TokenManager>();
        this.actionSystem = ServiceLocator.Get<ActionSystem>();

        isPreviewHovered = false;
    }

    void Update()
    {
        // UI가 있는 경우 
        if (EventSystem.current.IsPointerOverGameObject())
        {
            ExitHover();
            return;
        }
        // Input.mousePosition의 경우 screen position을 반환하기 때문에 world 좌표계 값으로 변경해줘야한다. 
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Collider를 탐지했을 경우 
        if (Physics.Raycast(ray, out hit, 300f))
        {
            if (hit.transform.gameObject.TryGetComponent<IHoverable>(out IHoverable hoverable))
            {
                // 새로운 IHoverable을 탐지했을 경우 
                if (hoverable != currentHoverable)
                {
                    // 기존의 IHoverable Exit
                    ExitHover();

                    // 새로 찾은 IHoverable 오브젝트가 hoverable한 경우 (카드의 경우, 선택되었거나, 카드가 움직이고 있거나, 액션을 하는 도중에는 onHover 상태가 되어서는 안됨) 
                    if (hoverable.IsHoverable())
                    {
                        EnterHover(hoverable);

                        hoverStartTime = Time.time;
                        isPreviewHovered = false; 
                    }
                }

                else
                {
                    // Preview 호출 
                    if(Time.time - hoverStartTime >= hoverPreviewDelay && !isPreviewHovered)
                    {
                        isPreviewHovered = true;

                        if (hit.transform.TryGetComponent<BaseObject>(out BaseObject baseObject))
                            onHoverStart?.Invoke(baseObject);

                        else if (hit.transform.TryGetComponent<GridCell>(out GridCell gridCell))
                        {
                            if (tokenManager == null)
                                return; 

                            if (tokenManager.TryGetTokenFrom(gridCell.GetGridPosition(), out Token token))
                                onHoverStart?.Invoke(token as BaseObject);
                        }

                        else
                            isPreviewHovered = false; 
                    }

                    if (currentHoverable != null && Time.time - hoverStartTime >= 0.5f)
                        HighlightHoveredAttackRange(hit.transform.position); 
                }
            }
        }

        // Collider를 탐지하지 못한 경우 
        else
        {
            if(currentHoverable != null)
                ExitHover();
        }
    }

    #region Hover
    public void EnterHover(IHoverable hoverable)
    {
        currentHoverable = hoverable;
        currentHoverable?.OnHover();
    }
    public void ExitHover()
    {
        if (currentHoverable == null)
            return; 

        currentHoverable?.OffHover();
        currentHoverable = null;

        onHoverExit?.Invoke();

        gridManager?.UnhighlightGridCells(HighlightLayer.Hover);
    }
    #endregion 
    
    public IHoverable GetCurrentHoverable() => currentHoverable;

    void HighlightHoveredAttackRange(Vector3 worldPosition)
    {
        if (currentHoverable == null)
            return;

        if (!actionSystem.IsActionInProgress())
            return;

        Vector2Int gridPosition = gridManager.GetGridPosition(worldPosition);
        
        if(tokenManager.TryGetTokenFrom(gridPosition, out Token token))
        {
            if (token.AttackRange == null || token.AttackRange.Count <= 0)
                return;

            IAction currentAction = actionSystem.GetCurrentAction();

            if (currentAction.ActionType == ActionType.Summon)
                return;

            BaseObject executor = currentAction?.Executor; 

            if (ReferenceEquals(token, executor))
                return;

            gridManager.UnhighlightGridCells(HighlightLayer.Hover); 
            gridManager.HighlightGridCells((Vector2Int gridPos) =>
            {
                foreach (var pos in token.AttackRange)
                {
                    if (gridPos == (gridPosition + pos))
                        return true;
                }

                return false; 

            }, HighlightType.HoverHighlight, HighlightLayer.Hover);
        }
    }

    public void EnableSystem() => enabled = true;
    public void DisableSystem()
    {
        enabled = false;
        ExitHover(); 
    }
}
