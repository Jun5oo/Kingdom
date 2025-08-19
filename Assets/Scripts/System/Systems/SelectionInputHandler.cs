using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectionInputHandler
{
    SelectionResolver resolver;
    Action<ISelectable> onSelect;

    public SelectionInputHandler(SelectionResolver resolver, Action<ISelectable> onSelect)
    {
        this.resolver = resolver; 
        this.onSelect = onSelect;
    }

    public void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            onSelect?.Invoke(null);
            return;
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 300f))
            {
                ISelectable selectable = resolver.Resolve(hit);
                onSelect?.Invoke(selectable); 
            }
        }
    }
}
