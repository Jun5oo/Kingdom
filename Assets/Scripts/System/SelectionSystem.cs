using System;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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

        this.actionSystem.OnCancelOccured += OnExitSelected; 
    }
    void Update()
    {
        if (currentSelectable != null)
            currentSelectableName = currentSelectable.ToString();
        else
            currentSelectableName = "Null";

        // 우클릭 시 선택취소 
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            OnExitSelected();
            return; 
        }

        // UI가 위에 가려져 있을 시 클릭되서는 안됨 
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100f))
            {
                ISelectable selectable;
                GridCell gridCell;

                // 직접 클릭한 경우 
                if (hit.transform.gameObject.TryGetComponent<ISelectable>(out selectable))
                    OnEnterSelected(selectable);

                // 간접적으로 클릭한 경우
                else if (hit.transform.gameObject.TryGetComponent<GridCell>(out gridCell))
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
        if (!selectable.IsSelectable())
            return;

        OnExitSelected(); 
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
