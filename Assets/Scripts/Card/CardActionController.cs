using System.Collections.Generic;
using UnityEngine;

public class CardActionController : MonoBehaviour
{
    IUISystem uiSystem;
    IActionSystem actionSystem;

    List<IAction> actions;
    List<ActionUI> actionUIList;

    [Header("Components")]
    [SerializeField] Card card; 
    [SerializeField] CardHover cardHover; 
    [SerializeField] Transform actionUIPoisition; 

    public void Init(IUISystem uiSystem, IActionSystem actionSystem, IGridSystem gridSystem)
    {
        this.uiSystem = uiSystem; 
        this.actionSystem = actionSystem;

        actions = new List<IAction>();
        actionUIList = new List<ActionUI>();

        foreach (ActionType actionType in card.Actions)
        {
            IAction action = actionSystem?.Create(gridSystem, card, actionType);
            actions.Add(action); 
        }

        cardHover.OnCardSelected -= ShowEnableActions;
        cardHover.OnCardDeselected -= HideEnableActions;
        cardHover.OnCardSelected += ShowEnableActions;
        cardHover.OnCardDeselected += HideEnableActions; 
    }

    public void ShowEnableActions()
    {
        Transform uiLayout = uiSystem.GetActionUIParent();
        uiLayout.position = Camera.main.WorldToScreenPoint(actionUIPoisition.position);

        foreach(IAction action in actions)
        {
            if (!action.IsValid())
                continue;

            // Temp 
            GameObject obj = uiSystem.Pop<ActionUI>(); 
            ActionUI actionUI = obj.GetComponent<ActionUI>();

            actionUI.Init(action);

            actionUI.OnUIClicked -= ActionUIClicked;
            actionUI.OnUIClicked += ActionUIClicked;

            actionUIList.Add(actionUI);
        }
    }
    public void HideEnableActions()
    {
        foreach(ActionUI actionUI in actionUIList)
        {
            actionUI.OnUIClicked -= ActionUIClicked;
            uiSystem.Push<ActionUI>(actionUI.gameObject); 
        }

        actionUIList.Clear(); 
    }
    public void ActionUIClicked(IAction action) => actionSystem?.EnterAction(action);
    public bool IsActionValid() => true;

    public void OnDestroy()
    {
        cardHover.OnCardSelected -= ShowEnableActions;
        cardHover.OnCardDeselected -= HideEnableActions;
    }
}
