using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 카드 액션을 처리하는 클래스.
/// 실행 가능한 액션을 UI로 표시 및 전달 
/// </summary>

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
            // 카드 데이터에 작성된 행동가능한 Action을 생성, 리스트에 저장 
            IAction action = actionSystem?.Create(gridSystem, card, actionType);
            actions.Add(action); 
        }

        cardHover.OnCardSelected -= ShowEnableActions;
        cardHover.OnCardDeselected -= HideEnableActions;
        cardHover.OnCardSelected += ShowEnableActions;
        cardHover.OnCardDeselected += HideEnableActions; 
    }

    /// <summary>
    /// 현재 사용가능한 Action을 UI로 표시 
    /// </summary>
    public void ShowEnableActions()
    {
        Transform uiLayout = uiSystem.GetActionUIParent();
        uiLayout.position = Camera.main.WorldToScreenPoint(actionUIPoisition.position);

        GameFlowManager gameFlowManager = GameObject.FindAnyObjectByType<GameFlowManager>();
        if (!gameFlowManager.IsMyTurn(card.IsMyCard))
            return;

        foreach (IAction action in actions)
        {
            if (!action.IsValid())
                continue;

            GameObject obj = uiSystem.Pop<ActionUI>(); 
            ActionUI actionUI = obj.GetComponent<ActionUI>();

            actionUI.Init(action);

            actionUI.OnUIClicked -= ActionUIClicked;
            actionUI.OnUIClicked += ActionUIClicked;

            actionUIList.Add(actionUI);
        }
    }

    /// <summary>
    /// 표시된 Action UI를 숨김 
    /// </summary>
    public void HideEnableActions()
    {
        foreach(ActionUI actionUI in actionUIList)
        {
            actionUI.OnUIClicked -= ActionUIClicked;
            uiSystem.Push<ActionUI>(actionUI.gameObject); 
        }

        actionUIList.Clear(); 
    }
    /// <summary>
    /// Action UI가 클릭되었을 때 ActionSystem에 해당 Action을 전달 
    /// </summary>
    /// <param name="action"></param> 전달할 Action 
    public void ActionUIClicked(IAction action) => actionSystem?.EnterAction(action);

    public void OnDestroy()
    {
        cardHover.OnCardSelected -= ShowEnableActions;
        cardHover.OnCardDeselected -= HideEnableActions;
    }
}
