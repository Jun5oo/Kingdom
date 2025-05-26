using System.Collections.Generic;
using UnityEngine;

public class CardActionController : MonoBehaviour
{
    IUISystem uiSystem;
    IActionSystem actionSystem; 

    List<IAction> actions;
    List<ActionUI> actionUIList;

    [SerializeField] CardHover cardHover; 
    [SerializeField] Transform actionUIPoisition; 

    public void Init(IUISystem uiSystem, IActionSystem actionSystem, IGridSystem gridSystem)
    {
        this.uiSystem = uiSystem; 
        this.actionSystem = actionSystem;

        // 추후에는 ActionList를 CardData로부터 받아와 초기화 시켜줄 예정 
        SummonAction summonAction = new SummonAction(gridSystem, actionSystem, this.gameObject);
        MoveAction moveAction = new MoveAction(gridSystem, actionSystem, this.gameObject);
        AttackAction attackAction = new AttackAction(gridSystem, actionSystem, this.gameObject);

        actions = new List<IAction>();
        actionUIList = new List<ActionUI>();

        actions.Add(summonAction);
        actions.Add(moveAction);
        actions.Add(attackAction);

        cardHover.OnCardSelected -= ShowEnableActions;
        cardHover.OnCardDeselected -= HideEnableActions;

        cardHover.OnCardSelected += ShowEnableActions;
        cardHover.OnCardDeselected += HideEnableActions; 
    }

    public void ShowEnableActions()
    {
        GameObject uiLayout = uiSystem.GetActionUIParent();
        uiLayout.transform.position = Camera.main.WorldToScreenPoint(actionUIPoisition.position);

        foreach(IAction action in actions)
        { 
            if (action.IsValid())
            {
                GameObject obj = uiSystem.PopActionUI();
                ActionUI actionUI = obj.GetComponent<ActionUI>();

                actionUI.Init(action);
                
                actionUI.OnUIClicked -= ActionUiClicked;
                actionUI.OnUIClicked += ActionUiClicked;

                actionUIList.Add(actionUI); 
            }
        }
    }
    public void HideEnableActions()
    {
        foreach(ActionUI actionUI in actionUIList)
        {
            actionUI.OnUIClicked -= ActionUiClicked;
            uiSystem.PushActionUI(actionUI.gameObject); 
        }

        actionUIList.Clear(); 
    }
    public void ActionUiClicked(IAction action) => actionSystem?.EnterAction(action);
    public bool IsActionValid() => true;

    public void OnDestroy()
    {
        cardHover.OnCardSelected -= ShowEnableActions;
        cardHover.OnCardDeselected -= HideEnableActions;
    }
}
