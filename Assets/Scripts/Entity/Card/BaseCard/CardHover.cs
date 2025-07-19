using System;
using UnityEngine;

public class CardHover : EntityHover
{
    [SerializeField] Card card;
    [SerializeField] CardMovement cardMovement;

    Vector3 hoverOffset;
    Vector3 hoverScale;

    Vector3 selectedOffset;
    Vector3 selectedScale;

    public Action<Card> OnCardSelected;
    public Action OnCardDeselected;

    public override Entity Entity { get { return card; } }
    public override void Init()
    {
        base.Init(); 

        cardMovement.OnMoved -= OnUpdatePRS; 
        cardMovement.OnMoved += OnUpdatePRS;
        
        cardMovement.OnMovedComplete -= OnMoveComplete;
        cardMovement.OnMovedComplete += OnMoveComplete;

        originScale = Vector3.one; 

        hoverOffset = Vector3.up * 0.01f;
        hoverScale = originScale * 1.1f;

        selectedOffset = Vector3.up * 0.5f + Vector3.forward * 0.5f;
        selectedScale = originScale * 1.3f;
    }

    #region Hover 
    public override void OnHover()
    {
        if (!IsHoverable())
            return; 

        Vector3 targetPosition = originPos + hoverOffset;
        Vector3 targetScale = hoverScale;

        cardMovement.MoveTransform(new PRS(targetPosition, originRotation, targetScale), 0f, true, () => { hoverState = HoverState.Hover; });
    }
    public override void OffHover()
    {
        if (hoverState != HoverState.Hover)
            return; 

        cardMovement.MoveTransform(new PRS(originPos, originRotation, originScale), 0f, true, () => { hoverState = HoverState.Idle; }); 
    }
    #endregion

    #region Selection
    public override void OnSelected()
    {
        if (!IsSelectable())
            return; 

        Vector3 targetPosition = originPos + selectedOffset;
        Quaternion targetRotation = originRotation;
        Vector3 targetScale = selectedScale;

        // 카드가 클릭된 것처럼 보이게 카드의 scale과 높이를 조정, 이후 카드가 선택되었음을 이벤트로 전달 
        cardMovement.MoveTransform(new PRS(targetPosition, targetRotation, targetScale), 0.2f, true, () => 
        {
            hoverState = HoverState.Selected; 
            OnCardSelected?.Invoke(card);
            OnSelectionComplete(); 
        });

        Debug.Log(hoverState); 
    }
    public override void OnDeselected()
    {
        if (hoverState != HoverState.Selected)
            return; 

        cardMovement.MoveTransform(new PRS(originPos, originRotation, originScale), 0.2f, true, ()=>
        {
            hoverState = HoverState.Idle;
            OnCardDeselected?.Invoke();
        });
    }
    #endregion

    public override void OnUpdatePRS()
    {
        base.OnUpdatePRS(); 

        PRS prs = cardMovement.PRS; 

        originPos = prs.position;
        originRotation = prs.rotation;
        originScale = prs.scale;
    }
    void OnDestroy()
    {
        UnSubscribe();

        cardMovement.OnMoved -= OnUpdatePRS;
        cardMovement.OnMovedComplete -= OnMoveComplete;
        OnCardSelected = null;
        OnCardDeselected = null;
    }
}
