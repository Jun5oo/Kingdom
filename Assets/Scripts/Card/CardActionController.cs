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
        
        // CardData로부터 Action을 받아올 예정 
        SummonAction summonAction = new SummonAction(gridSystem, actionSystem, card);
        MoveAction moveAction = new MoveAction(gridSystem, actionSystem, card);
        AttackAction attackAction = new AttackAction(gridSystem, actionSystem, card);
        // KingSummonAction kingSummonAction = new KingSummonAction(gridSystem, actionSystem, card);

        actions = new List<IAction>();
        actionUIList = new List<ActionUI>();

        actions.Add(summonAction);
        actions.Add(moveAction);
        actions.Add(attackAction);
        // actions.Add(kingSummonAction);

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

            // 왕 카드이면 UI를 생성하지 않음
            if (card != null && card.IsKing)
                continue;

            // Temp 
            GameObject obj = uiSystem.Pop<ActionUI>(); 
            ActionUI actionUI = obj.GetComponent<ActionUI>();

            actionUI.Init(action);

            actionUI.OnUIClicked -= ActionUiClicked;
            actionUI.OnUIClicked += ActionUiClicked;

            actionUIList.Add(actionUI);
        }
    }
    public void HideEnableActions()
    {
        foreach(ActionUI actionUI in actionUIList)
        {
            actionUI.OnUIClicked -= ActionUiClicked;
            uiSystem.Push<ActionUI>(actionUI.gameObject); 
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
