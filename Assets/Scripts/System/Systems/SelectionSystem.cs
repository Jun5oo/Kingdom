using System;
using UnityEngine;
using UnityEngine.UI;

public class SelectionSystem : MonoBehaviour, IGameSystem
{
    ISelectable currentSelectable;

    SelectionResolver resolver;
    SelectionInputHandler handler;

    TurnSystem turnSystem; 

    // ActionDisplayer에 전달 
    public Action<BaseObject> onSelectedComplete;
    public Action onDeselected;

    bool isSelectionLocked; 

    void Awake()
    {
        DisableSystem();
    }

    public void Init()
    {
        this.currentSelectable = null;

        this.resolver = new SelectionResolver();
        this.handler = new SelectionInputHandler(resolver, TrySelect);

        this.turnSystem = ServiceLocator.Get<TurnSystem>(); 
    }
    void Update()
    {
        handler.Update(); 
    }

    public void TrySelect(ISelectable selectable)
    {
        if (isSelectionLocked)
        {
            Debug.Log("SelectionLocked 상태입니다."); 
            return;
        }

        if (selectable == currentSelectable)
        {
            Debug.Log("selectable == currentSelectable"); 
            return;
        }

        if (!resolver.IsValid(selectable))
        {
            Debug.Log($"{selectable}은 Valid하지 않은 상태입니다.");
            OnExitSelected();
            return;
        }

        OnExitSelected(); 
        OnEnterSelected(selectable); 
    }

    public void OnEnterSelected(ISelectable selectable)
    {
        if (selectable == null)
            return;

        Debug.Log("Check"); 
        isSelectionLocked = true; 

        currentSelectable = selectable;

        currentSelectable.OnSelectedComplete -= OnSelectedComplete;
        currentSelectable.OnSelectedComplete += OnSelectedComplete;
        
        currentSelectable.OnSelected();
    }

    public void OnSelectedComplete()
    {
        if (currentSelectable == null)
            return;

        isSelectionLocked = false; 

        BaseObject baseObject = currentSelectable.BaseObject; 

        if (baseObject == null)
            return;

        int currentTurnPlayer = turnSystem.GetCurrentTurnPlayerID();

        // Display Playable Actions 
        if (baseObject.OwnerID == currentTurnPlayer)
            onSelectedComplete?.Invoke(baseObject);
    }

    public void OnExitSelected()
    {
        onDeselected?.Invoke();

        if (currentSelectable != null)
            currentSelectable.OnSelectedComplete -= OnSelectedComplete;

        // currentSelectable이 null인 경우도 생각
        currentSelectable?.OnDeselected();
        currentSelectable = null;

        isSelectionLocked = false; 
    }

    public ISelectable GetCurrentSelected() => currentSelectable; 

    public void EnableSystem()
    {
        enabled = true;
        isSelectionLocked = false; 
    }
    public void DisableSystem()
    {
        enabled = false;
        isSelectionLocked = true; 
    }

}
