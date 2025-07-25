using System;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class SelectionSystem : MonoBehaviour, IGameSystem
{
    ISelectable currentSelectable;

    SelectionResolver resolver;
    SelectionInputHandler handler;

    public Action<BaseObject> onSelected;
    public Action<BaseObject> onSelectedComplete;
    public Action onDeselected;

    public void Init()
    {
        this.resolver = new SelectionResolver();
        this.handler = new SelectionInputHandler(resolver, TrySelect);
        
        DisableSystem(); 

        currentSelectable = null;
    }
    void Update()
    {
        handler.Update(); 
    }

    public void TrySelect(ISelectable selectable)
    {
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

        onSelected?.Invoke(selectable.BaseObject); 

        currentSelectable = selectable;
        currentSelectable.OnSelected();
        
        currentSelectable.OnSelectedComplete -= OnSelectedComplete;
        currentSelectable.OnSelectedComplete += OnSelectedComplete; 
    }
    public void OnSelectedComplete()
    {
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
    }

    public void EnableSystem() => enabled = true; 
    public void DisableSystem() => enabled = false;

}
