using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 플레이어가 클릭한 오브젝트(카드/토큰)의 선택 상태를 관리하는 시스템.
/// SelectionInputHandler로 입력을 받고, SelectionResolver로 유효성을 검증한다.
/// 선택이 완료되면 onSelectedComplete 이벤트로 ActionDisplayer에 알린다.
/// </summary>
public class SelectionSystem : MonoBehaviour, IGameSystem
{
    ISelectable currentSelectable; // 현재 선택된 오브젝트

    SelectionResolver resolver;       // 레이캐스트 결과를 ISelectable로 변환
    SelectionInputHandler handler;    // 마우스 입력 처리

    PlayerManager playerManager;
    TurnSystem turnSystem;

    public Action<BaseObject> onSelectedComplete; // 선택 완료 시 ActionDisplayer에 전달
    public Action onDeselected;                   // 선택 해제 시 발생

    bool isSelectionLocked; // 선택 처리 중 중복 입력 방지 플래그

    void Awake()
    {
        DisableSystem();
    }

    public void Init()
    {
        this.currentSelectable = null;

        this.resolver = new SelectionResolver();
        this.handler = new SelectionInputHandler(resolver, TrySelect);

        this.playerManager = ServiceLocator.Get<PlayerManager>();
        this.turnSystem = ServiceLocator.Get<TurnSystem>();
    }
    void Update()
    {
        handler.Update();
    }

    /// <summary>
    /// 입력된 ISelectable의 선택을 시도한다.
    /// 잠금 상태이거나 유효하지 않으면 무시하고, 기존 선택을 해제한 뒤 새 선택을 진입한다.
    /// </summary>
    public void TrySelect(ISelectable selectable)
    {
        if (isSelectionLocked)
        {
            Debug.Log("SelectionLocked 상태입니다.");
            return;
        }

        if (selectable == currentSelectable)
        {
            Debug.Log("selectable == currentSelectable");
            return;
        }

        if (!resolver.IsValid(selectable))
        {
            Debug.Log($"{selectable}은 Valid하지 않은 상태입니다.");
            OnExitSelected();
            return;
        }

        OnExitSelected();
        OnEnterSelected(selectable);
    }

    /// <summary> 새로운 오브젝트 선택 진입. 선택 완료 이벤트를 구독하고 OnSelected를 호출한다. </summary>
    public void OnEnterSelected(ISelectable selectable)
    {
        if (selectable == null)
            return;

        isSelectionLocked = true;
        currentSelectable = selectable;

        currentSelectable.OnSelectedComplete -= OnSelectedComplete;
        currentSelectable.OnSelectedComplete += OnSelectedComplete;

        currentSelectable.OnSelected();
    }

    /// <summary>
    /// 선택 완료 콜백. 로컬 플레이어의 턴이고 자신의 오브젝트일 때만
    /// onSelectedComplete를 통해 ActionDisplayer에 액션 팝업을 요청한다.
    /// </summary>
    public void OnSelectedComplete()
    {
        if (currentSelectable == null)
            return;

        isSelectionLocked = false;

        BaseObject baseObject = currentSelectable.BaseObject;

        if (baseObject == null)
            return;

        if (baseObject.OwnerID == playerManager.Local.PlayerID && turnSystem.GetCurrentTurnPlayerID() == playerManager.Local.PlayerID)
            onSelectedComplete?.Invoke(baseObject);
    }

    /// <summary> 현재 선택을 해제하고 모든 상태를 초기화한다. </summary>
    public void OnExitSelected()
    {
        onDeselected?.Invoke();

        if (currentSelectable != null)
            currentSelectable.OnSelectedComplete -= OnSelectedComplete;

        currentSelectable?.OnDeselected();
        currentSelectable = null;
        isSelectionLocked = false;
    }

    public ISelectable GetCurrentSelected() => currentSelectable;

    public void EnableSystem()
    {
        enabled = true;
        isSelectionLocked = false;
    }
    public void DisableSystem()
    {
        enabled = false;
        isSelectionLocked = true;
    }

}
