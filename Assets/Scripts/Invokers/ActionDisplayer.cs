using System.Collections.Generic;
using UnityEngine;

public class ActionDisplayer : MonoBehaviour
{
    // 액션 팝업 Displayer. ISelectable한 BaseObject을 클릭했을 때 해당 오브젝트가 할 수 있는 행동을 Popup으로 보여줌 
    PoolManager poolManager;
    SelectionSystem selectionSystem; 
    ActionFactory actionFactory;
    ActionSystem actionSystem; 
    
    List<ActionPopup> pooled;

    [SerializeField] Transform layout; 

    void Start()
    {
        poolManager = ServiceLocator.Get<PoolManager>();
        selectionSystem = ServiceLocator.Get<SelectionSystem>();
        actionFactory = ServiceLocator.Get<ActionFactory>();
        actionSystem = ServiceLocator.Get<ActionSystem>();

        pooled = new List<ActionPopup>();

        selectionSystem.onSelectedComplete -= Display; 
        selectionSystem.onSelectedComplete += Display;

        selectionSystem.onDeselected -= Clear;
        selectionSystem.onDeselected += Clear;
    }

    public void Display(BaseObject baseObject)
    {
        if (layout != null)
        {
            if (baseObject.TryGetComponent<BaseView>(out BaseView view))
                layout.transform.position = Camera.main.WorldToScreenPoint(view.Anchor.position);
        }

        else
        {
            Debug.LogError("Layout not found");
            return; 
        }

        var actionTypes = baseObject.Actions;
        Debug.Log(actionTypes); 

        foreach (var actionType in actionTypes)
        {
            IAction action = actionFactory.CreateAction(actionType, baseObject);
            Debug.Log(action); 

            if (action != null)
            {
                if (action.IsValid())
                {
                    ActionPopup actionPopup = poolManager.Pop<ActionPopup>(); 
                    actionPopup.Init(action);
                    actionPopup.gameObject.transform.SetParent(layout.transform, false);

                    actionPopup.OnSelected -= actionSystem.Enter;
                    actionPopup.OnSelected += actionSystem.Enter; 

                    pooled.Add(actionPopup);
                }
            }
        }
    }
    public void Clear()
    {
        foreach (var action in pooled)
        {
            action.OnSelected -= actionSystem.Enter; 
            action.OnSelected = null;

            poolManager.Push(action);
        }


        pooled.Clear(); 
    }
    void OnDestroy()
    {
        Clear();
    }
 
}
