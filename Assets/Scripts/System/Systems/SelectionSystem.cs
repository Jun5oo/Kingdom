using System;
using UnityEngine;

public class SelectionSystem : MonoBehaviour, IGameSystem
{
    ISelectable currentSelectable;

    SelectionResolver resolver;
    SelectionInputHandler handler;

    // Preview Displayer에 전달 
    public Action<BaseObject> onSelected;
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
        this.resolver = new SelectionResolver();
        this.handler = new SelectionInputHandler(resolver, TrySelect);

        currentSelectable = null;
    }
    void Update()
    {
        handler.Update(); 
    }

    public void TrySelect(ISelectable selectable)
    {
        if (isSelectionLocked)
            return; 

        if (selectable == currentSelectable)
            return;

        if (!resolver.IsValid(selectable))
        {
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

        isSelectionLocked = true; 

        onSelected?.Invoke(selectable.BaseObject); 

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
