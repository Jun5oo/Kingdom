using System;
using UnityEngine;
using UnityEngine.UI;

public class SelectionSystem : MonoBehaviour
{
    ISelectable currentSelectable = null;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            OnExitSelected();
            return; 
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 50f) && Input.GetKeyDown(KeyCode.Mouse0))
        {
            ISelectable selectable;

            if (hit.transform.gameObject.TryGetComponent<ISelectable>(out selectable))
            {
                if(currentSelectable != selectable && selectable.IsSelectable())
                {
                    OnExitSelected(); 
                    currentSelectable = selectable;
                    OnEnterSelected(); 
                }
            }
        }
    }

    void OnEnterSelected()
    {
        currentSelectable?.OnSelected();
    }

    void OnExitSelected() => currentSelectable?.OnDeselected(); 

    public ISelectable GetCurentSelectable() => currentSelectable;
}
