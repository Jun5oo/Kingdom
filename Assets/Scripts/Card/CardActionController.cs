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

        // ���Ŀ��� ActionList�� CardData�κ��� �޾ƿ� �ʱ�ȭ ������ ���� 
        SummonAction summonAction = new SummonAction(gridSystem, actionSystem, this.gameObject);
        MoveAction moveAction = new MoveAction(gridSystem, actionSystem, this.gameObject);
        AttackAction attackAction = new AttackAction(gridSystem, actionSystem, this.gameObject);
        KingSummonAction kingSummonAction = new KingSummonAction(gridSystem, actionSystem, this.gameObject);

        actions = new List<IAction>();
        actionUIList = new List<ActionUI>();

        actions.Add(summonAction);
        actions.Add(moveAction);
        actions.Add(attackAction);
        actions.Add(kingSummonAction);

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
            if (!action.IsValid())
                continue;

            // 왕 카드이면 UI를 생성하지 않음
            Card card = this.gameObject.GetComponent<Card>();
            if (card != null && card.IsKing)
                continue;

            GameObject obj = uiSystem.PopActionUI();
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
