using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UIInvoker 
{
    UIManager uiSystem;
    ActionSystem actionSystem; 
    ActionFactory actionFactory;

    List<ActionUI> pooledActionUI;

    public Action OnActionUISelected; 

    public void Init()
    {
        this.uiSystem = ServiceLocator.Get<UIManager>();
        this.actionSystem = ServiceLocator.Get<ActionSystem>();
        this.actionFactory = ServiceLocator.Get<ActionFactory>(); 

        pooledActionUI = new List<ActionUI>();
    }

    public void DisplayPreviewUI(Entity entity) => uiSystem?.DisplayUI(entity);
    public void ClosePreviewUI() => uiSystem?.CloseUI();
    public void DisplayActionUI(Entity entity, Transform transform)
    {
        // 이 경우, 카드가 선택되었을 경우이므로 기존에 선택되어 보여지는 CardUI와 CardActionUI들은 모두 비활성화 시켜줘야한다. 
        ClearActionUI();

        var actionTypes = entity.Actions;
        Debug.Log(actionTypes.Count); 
        var sorted = actionTypes.OrderBy(a => (int)a).ToList();

        foreach (var actionType in sorted)
        {
            // CardAction을 생성 
            IAction action = actionFactory.CreateAction(actionType, entity);

            if (action != null)
            {
                if (action.IsValid())
                {
                    // 실행할 수 있는 CardAction을 UI로 표시 (CardActionUI 생성) 

                    Transform layout = uiSystem?.ActionUILayout; 
                    
                    if(layout == null)
                    {
                        Debug.LogError("ActionUILayout not found");
                        return; 
                    }

                    layout.transform.position = Camera.main.WorldToScreenPoint(transform.position); 

                    GameObject ActionUIObject = uiSystem.Pop<ActionUI>();
                    ActionUI ActionUI = ActionUIObject.GetComponent<ActionUI>();
                    ActionUI.Init(action);

                    ActionUI.OnSelected -= OnSelected; 
                    ActionUI.OnSelected += OnSelected;

                    pooledActionUI.Add(ActionUI);
                }
            }
        }
    }
    public void ClearActionUI()
    {
        foreach(var ActionUI in pooledActionUI)
        {
            ActionUI.OnSelected -= OnSelected;
            uiSystem.Push<ActionUI>(ActionUI.gameObject); 
        }

        pooledActionUI.Clear(); 
    }

    void OnSelected(IAction action)
    {
        // SelectionSystem에게 이벤트 전달 
        OnActionUISelected?.Invoke();
        actionSystem?.Enter(action);
    }

    void OnDestroy()
    {
        ClearActionUI(); 
    }

}
