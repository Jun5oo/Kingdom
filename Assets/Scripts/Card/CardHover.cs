using System;
using UnityEngine;

/// <summary>
/// 카드의 Hover와 Selection(마우스 Interaction)을 처리하는 클래스. 
/// 현재는 Hover와 Selection을 둘 다 처리하지만, 추후 코드가 늘어난다면 분리 할 예정 
/// </summary>

public class CardHover : MonoBehaviour, IHoverable, ISelectable
{
    [Header("Card Components")]
    [SerializeField] Card card; 
    [SerializeField] CardMovement cardMovement;

    [Header("System Referecne")]
    IUISystem uiSystem;

    [SerializeField] Vector3 originPos;
    [SerializeField] Quaternion originRotation;
    [SerializeField] Vector3 originScale;

    // OnHover PRS 
    [SerializeField] Vector3 hoverOffset;
    [SerializeField] Vector3 hoverScale;

    // OnSelected PRS 
    Vector3 selectedOffset;
    Vector3 selectedScale;

    [SerializeField] bool isHoverable = false;
    [SerializeField] bool isSelected = false;

    // 카드가 선택되었을 때 
    public Action OnCardSelected;
    // 카드의 선택이 취소되었을 때 
    public Action OnCardDeselected; 

    // TODO 
    // 적 카드가 필드에서 선택되었을 때 뒤집어지는 문제 

    public void Init(IUISystem uiSystem)
    {
        this.uiSystem = uiSystem;

        cardMovement.OnCardMoved -= OnPRSUpdate; 
        cardMovement.OnCardMoved += OnPRSUpdate;
        // 카드가 움직이고 있을 때는 Hover가 되어서는 안된다. (드로우를 하거나, 카드를 소환, 이동할 때 마우스가 해당 경로에 있으면 HoverSystem이 발동해서는 안된다) 
        cardMovement.OnCardMovedComplete -= OnHoverEnable; 
        cardMovement.OnCardMovedComplete += OnHoverEnable;

        originScale = card.IsMyCard ? Vector3.one : Vector3.one * 2; 

        hoverOffset = Vector3.up * 0.01f;
        hoverScale = originScale * 1.1f;

        selectedOffset = Vector3.up * 0.5f + Vector3.forward * 0.5f;
        selectedScale = originScale * 1.3f;
    }

    #region Hover 
    public void OnHover()
    {
        if (!IsHoverable())
            return;

        Vector3 targetPosition = originPos + hoverOffset;
        Vector3 targetScale = hoverScale; 

        cardMovement.MoveTransform(new PRS(targetPosition, originRotation, targetScale), 0f, true);
    }
    public void OffHover()
    {
        if (!IsHoverable())
            return;
        
        cardMovement.MoveTransform(new PRS(originPos, originRotation, originScale), 0f, true); 
    }
    public bool IsHoverable()
    {
        if (!isHoverable || isSelected || cardMovement.IsMoving())
            return false;

        return true; 
    }
    #endregion

    #region Selection
    public void OnSelected()
    {
        if (!IsSelectable())
            return;

        // 선택된 카드를 UI로 표시 
        uiSystem?.DisplayUI(card);

        Vector3 targetPosition = originPos + selectedOffset;
        Quaternion targetRotation = originRotation; 
        Vector3 targetScale = card.CardState == CardState.Hand ? selectedScale : originScale * 1.1f; 

        // 카드가 클릭된 것처럼 보이게 카드의 scale과 높이를 조정, 이후 카드가 선택되었음을 이벤트로 전달 
        cardMovement.MoveTransform(new PRS(targetPosition, targetRotation, targetScale), 0.2f, true, () => 
        { 
            OnCardSelected?.Invoke();
        });
        isSelected = true;
    }
    public void OnDeselected()
    {
        uiSystem?.CloseUI(); 

        if(!cardMovement.IsMoving())
            cardMovement.MoveTransform(new PRS(originPos, originRotation, originScale), 0.2f, true);

        isSelected = false;
        OnCardDeselected?.Invoke(); 
    }
    public bool IsSelectable()
    {
        if (isSelected || cardMovement.IsMoving())
            return false;

        return true; 
    }
    #endregion

    /// <summary>
    /// CardMovement로부터 PRS를 업데이트 
    /// </summary>
    /// <param name="prs"> 업데이트할 PRS </param> 
    public void OnPRSUpdate(PRS prs)
    {
        if (prs != null)
        {
            originPos = prs.position; 
            originRotation = prs.rotation;
            originScale = prs.scale;
        }
    }

    public void OnHoverEnable() => isHoverable = true; 

}
