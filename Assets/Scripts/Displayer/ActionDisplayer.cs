using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;

/// <summary>
/// 오브젝트를 선택했을 때 수행 가능한 ActionType 팝업 버튼을 표시하는 컴포넌트.
/// SelectionSystem의 onSelectedComplete 이벤트를 구독하고, ActionResolver로 유효한 액션을 조회한 뒤
/// ActionPopup을 풀에서 꺼내 레이아웃에 배치한다. 선택 해제 시 Clear()로 팝업을 반납한다.
/// </summary>
public class ActionDisplayer : MonoBehaviour
{
    PoolManager poolManager;

    SelectionSystem selectionSystem;
    ActionFactory actionFactory;
    ActionSystem actionSystem;

    ActionResolver actionResolver;

    List<ActionPopup> pooled;

    [SerializeField] Transform layout;

    void OnDisable()
    {
        selectionSystem.onSelectedComplete -= Display;
        selectionSystem.onDeselected -= Clear;
    }

    void Start()
    {
        poolManager = ServiceLocator.Get<PoolManager>();
        selectionSystem = ServiceLocator.Get<SelectionSystem>();
        actionFactory = ServiceLocator.Get<ActionFactory>();
        actionSystem = ServiceLocator.Get<ActionSystem>();

        actionResolver = ServiceLocator.Get<ActionResolver>();

        pooled = new List<ActionPopup>();

        selectionSystem.onSelectedComplete -= Display;
        selectionSystem.onSelectedComplete += Display;

        selectionSystem.onDeselected -= Clear;
        selectionSystem.onDeselected += Clear;
    }

    /// <summary>
    /// 선택된 오브젝트의 유효한 액션 팝업 버튼을 레이아웃에 배치한다.
    /// IsValid()와 CanPerformAction()을 모두 통과한 액션만 표시한다.
    /// </summary>
    public void Display(BaseObject baseObject)
    {
        Clear();

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

        var actionTypes = actionResolver.GetValidActions(baseObject);

        // CardData에 정의된 특수 능력 액션도 목록에 추가한다
        if (baseObject is Token && baseObject.Data.Action.Count > 0)
        {
            foreach (var actionType in baseObject.Data.Action)
            {
                if (actionType != ActionType.None)
                    actionTypes.Add(actionType);
            }
        }

        foreach (var actionType in actionTypes)
        {
            IAction action = actionFactory.CreateAction(actionType, baseObject);

            if (action == null)
                continue;

            if (!action.IsValid())
            {
                Debug.Log($"{action} invalid");
                continue;
            }

            if (!actionSystem.CanPerformAction(action, baseObject.OwnerID))
            {
                Debug.Log($"cannot perform {action}");
                continue;
            }

            ActionPopup actionPopup = poolManager.Pop<ActionPopup>();
            actionPopup.Init(action);
            actionPopup.gameObject.transform.SetParent(layout.transform, true);

            actionPopup.OnSelected -= actionSystem.Enter;
            actionPopup.OnSelected += actionSystem.Enter;

            actionPopup.OnClicked -= Clear;
            actionPopup.OnClicked += Clear;

            pooled.Add(actionPopup);
        }
    }

    /// <summary> 표시 중인 ActionPopup을 모두 이벤트 해제 후 풀에 반납한다. </summary>
    public void Clear()
    {
        foreach (var action in pooled)
        {
            action.OnSelected -= actionSystem.Enter;
            action.OnClicked -= Clear;
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
