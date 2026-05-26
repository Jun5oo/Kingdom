using UnityEngine;

/// <summary>
/// 카드의 Hover(살짝 위로)/Select(앞으로 돌출) 인터랙션을 처리한다.
/// isHover=true로 MoveTransform을 호출하여 PRS 상태를 건드리지 않고 시각적으로만 이동한다.
/// </summary>
public class CardInteraction : BaseInteraction
{
    [SerializeField] CardMovement movement;

    Vector3 hoverOffset;   // Hover 시 원래 위치에서의 오프셋
    Vector3 hoverScale;    // Hover 시 크기 배율

    Vector3 selectedOffset; // Select 시 원래 위치에서의 오프셋
    Vector3 selectedScale;  // Select 시 크기 배율

    void OnDisable()
    {
        Clear();
    }

    /// <summary> 이동 이벤트를 구독하고 Hover/Select 오프셋·스케일을 초기화한다. </summary>
    public override void Init(BaseObject baseObject)
    {
        base.Init(baseObject);

        movement.OnMoved -= OnUpdatePRS;
        movement.OnMoved += OnUpdatePRS;

        movement.OnMovedComplete -= OnMoveComplete;
        movement.OnMovedComplete += OnMoveComplete;

        originScale = Vector3.one;

        hoverOffset = Vector3.up * 0.01f;
        hoverScale = originScale * 1.1f;

        selectedOffset = Vector3.up * 0.5f + Vector3.forward * 0.5f;
        selectedScale = originScale * 1.3f;
    }

    #region Hover
    /// <summary> Hover 상태로 카드를 살짝 위로 이동하고 1.1배 확대한다. </summary>
    public override void OnHover()
    {
        if (!IsHoverable())
            return;

        Vector3 targetPosition = originPos + hoverOffset;
        Vector3 targetScale = hoverScale;

        movement.MoveTransform(new PRS(targetPosition, originRotation, targetScale), 0f, true, () => { currentState = InteractionState.Hover; });
    }

    /// <summary> Hover 해제 시 원래 위치·크기로 복귀한다. </summary>
    public override void OffHover()
    {
        if (currentState != InteractionState.Hover)
            return;

        movement.MoveTransform(new PRS(originPos, originRotation, originScale), 0f, true, () => { currentState = InteractionState.Idle; });
    }
    #endregion

    #region Selection
    /// <summary>
    /// 카드를 앞으로 돌출시켜 선택 상태임을 시각적으로 표현한다.
    /// 애니메이션 완료 후 OnSelectionComplete를 발생시킨다.
    /// </summary>
    public override void OnSelected()
    {
        if (!IsSelectable())
            return;

        Vector3 targetPosition = originPos + selectedOffset;
        Quaternion targetRotation = originRotation;
        Vector3 targetScale = selectedScale;

        movement.MoveTransform(new PRS(targetPosition, targetRotation, targetScale), 0.2f, true, () =>
        {
            currentState = InteractionState.Selected;
            OnSelectionComplete();
        });
    }

    public override void OnDeselected()
    {
        if (currentState != InteractionState.Selected)
            return;

        movement.MoveTransform(new PRS(originPos, originRotation, originScale), 0.2f, true, () => { currentState = InteractionState.Idle; });
    }
    #endregion

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
