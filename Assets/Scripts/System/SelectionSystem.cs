using System;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 선택을 관리하는 System 클래스.
/// </summary>

public class SelectionSystem : MonoBehaviour, ISelectionSystem
{
    ISelectable currentSelectable = null;
    IGridSystem gridSystem; 
    IActionSystem actionSystem;

    [SerializeField] string currentSelectableName; 

    public void Init(IGridSystem gridSystem, IActionSystem actionSystem)
    {
        this.gridSystem = gridSystem;
        this.actionSystem = actionSystem;
    }
    void Update()
    {
        if (currentSelectable != null)
            currentSelectableName = currentSelectable.ToString();
        else
            currentSelectableName = "Null";

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            OnExitSelected();
            return; 
        }

        // UI에 가려져 있는 경우
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (actionSystem.IsActionInProgress())
            {
                IAction current = (actionSystem as ActionSystem)?.GetCurrentAction();
                if (current != null && current.GetType().Name == "KingSummonAction")
                    return;

                if(current.ActionType == ActionType.Attack)
                {
                    OnExitSelected();
                    return;
                }
            }

            if (Physics.Raycast(ray, out hit, 100f))
            {
                // 카드를 직접 클릭한 경우 (패에 위치해있을 때) 
                if (hit.transform.gameObject.TryGetComponent<ISelectable>(out ISelectable selectable))
                    OnEnterSelected(selectable);

                // 간접으로 클릭한 경우 (필드위에 있을 때) 
                else if (hit.transform.gameObject.TryGetComponent<GridCell>(out GridCell gridCell))
                {
                    Vector2Int gridPosition = gridCell.GetGridPosition();

                    if (gridSystem.IsObjectOnGridPosition(gridPosition))
                    {
                        selectable = gridSystem.GetGameObjectOnGrid(gridPosition).GetComponent<ISelectable>();
                        OnEnterSelected(selectable);
                    }
                    else
                        OnExitSelected();
                }

                else
                    OnExitSelected();
            }
            else
                OnExitSelected(); 
        }
    }

    #region Selection
    public void OnEnterSelected(ISelectable selectable)
    {
        OnExitSelected();

        if (!selectable.IsSelectable())
            return;

        currentSelectable = selectable;
        currentSelectable?.OnSelected();
    }
    public void OnExitSelected()
    {
        if (actionSystem.IsActionInProgress())
            actionSystem?.CancelAction();
 
        currentSelectable?.OnDeselected();
        currentSelectable = null; 
    }
    #endregion 

    public ISelectable GetCurentSelectable() => currentSelectable;
}
