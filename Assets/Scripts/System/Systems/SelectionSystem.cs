using UnityEngine;
using UnityEngine.EventSystems;

public class SelectionSystem : MonoBehaviour, IGameSystem
{
    TurnManager turnManager;
    TokenManager tokenManager;
    ActionSystem actionSystem;
    UIInvoker uiInvoker;

    ISelectable currentSelectable;

    public void Init()
    {
        this.turnManager = ServiceLocator.Get<TurnManager>();
        this.tokenManager = ServiceLocator.Get<TokenManager>();
        this.actionSystem = ServiceLocator.Get<ActionSystem>(); 
        this.uiInvoker = ServiceLocator.Get<UIInvoker>();

        DisableSystem(); 

        uiInvoker.OnActionUISelected -= OnExitSelected;
        uiInvoker.OnActionUISelected += OnExitSelected;

        currentSelectable = null;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            OnExitSelected();
            return;
        }

        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100f))
            {
                if (hit.transform.gameObject.TryGetComponent<ISelectable>(out ISelectable direct))
                {
                    if (direct != currentSelectable)
                        OnEnterSelected(direct);
                }
                else if (hit.transform.gameObject.TryGetComponent<GridCell>(out GridCell cell))
                {
                    Vector2Int gridPosition = cell.GetGridPosition();

                    if (tokenManager.TryGetTokenFrom(gridPosition, out Token token))
                    {
                        if (token == null)
                        {
                            OnExitSelected();
                            return;
                        }

                        if (token.TryGetComponent<ISelectable>(out ISelectable indirect))
                        {
                            if (indirect != currentSelectable)
                                OnEnterSelected(indirect);
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
            else
                OnExitSelected();
        }
    }

    public void OnEnterSelected(ISelectable selectable)
    {
        OnExitSelected();

        if (selectable.Entity != null)
            uiInvoker.DisplayPreviewUI(selectable.Entity);

        if (turnManager.GetCurrentTurnPlayerID() != selectable.Entity.OwnerPlayerID)
            return;

        if (!selectable.IsSelectable())
            return;

        if (actionSystem.IsActionInProgress())
            return; 

        currentSelectable = selectable;
        currentSelectable?.OnSelected();
        
        currentSelectable.OnSelectedComplete -= OnSelectedComplete;
        currentSelectable.OnSelectedComplete += OnSelectedComplete; 
    }
    public void OnSelectedComplete()
    {
        if (currentSelectable.Entity != null)
        {
            Entity entity = currentSelectable.Entity;
            if (entity.TryGetComponent<EntityView>(out EntityView view))
                uiInvoker.DisplayActionUI(entity, view.AnchorUI);
        }
    }
    public void OnExitSelected()
    {
        uiInvoker.ClosePreviewUI();
        uiInvoker.ClearActionUI();

        if(currentSelectable != null)
            currentSelectable.OnSelectedComplete -= OnSelectedComplete; 

        currentSelectable?.OnDeselected();
        currentSelectable = null;
    }
    public ISelectable GetCurrentSelectable() => currentSelectable;

    public void EnableSystem()
    {
        this.enabled = true; 
    }
    public void DisableSystem()
    {
        this.enabled = false; 
    }
}
