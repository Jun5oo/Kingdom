using UnityEngine;

/// <summary>
/// 토큰의 Hover/Select/Move 인터랙션을 처리한다.
/// TokenMovement의 OnMoved/OnMovedComplete 이벤트를 구독하여 이동 완료 시 상태를 자동 복귀시킨다.
/// </summary>
public class TokenInteraction : BaseInteraction
{
    [SerializeField] TokenMovement movement;

    /// <summary>
    /// 이동 이벤트를 구독하고 현재 PRS를 origin으로 저장한다.
    /// 중복 구독을 방지하기 위해 먼저 해제한 뒤 다시 등록한다.
    /// </summary>
    public override void Init(BaseObject baseObject)
    {
        base.Init(baseObject);

        movement.OnMoved -= OnUpdatePRS;
        movement.OnMoved += OnUpdatePRS;

        movement.OnMovedComplete -= OnMoveComplete;
        movement.OnMovedComplete += OnMoveComplete;

        originPos = movement.PRS.position;
        originRotation = movement.PRS.rotation;
        originScale = movement.PRS.scale;
    }

    public override void OnHover()
    {
        if (!IsHoverable())
            return;

        currentState = InteractionState.Hover;
    }

    public override void OffHover()
    {
        if (currentState != InteractionState.Hover)
            return;

        currentState = InteractionState.Idle;
    }

    /// <summary> 선택 가능 상태이면 Selected로 전환하고 OnSelectionComplete를 발생시킨다. </summary>
    public override void OnSelected()
    {
        if (!IsSelectable())
            return;

        currentState = InteractionState.Selected;
        OnSelectionComplete();
    }

    public override void OnDeselected()
    {
        if (currentState != InteractionState.Selected)
            return;

        currentState = InteractionState.Idle;
    }

    /// <summary> 이동 시작 시 Moving 상태로 전환하고 origin PRS를 현재 값으로 갱신한다. </summary>
    public override void OnUpdatePRS()
    {
        base.OnUpdatePRS();

        PRS prs = movement.PRS;

        originPos = prs.position;
        originRotation = prs.rotation;
        originScale = prs.scale;
    }

    void OnDestroy()
    {
        Clear();
    }

    void Clear()
    {
        UnSubscribe();
        movement.OnMoved -= OnUpdatePRS;
        movement.OnMovedComplete -= OnMoveComplete;
    }
}
