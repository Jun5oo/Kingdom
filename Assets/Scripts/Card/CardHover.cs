using System;
using UnityEngine;

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
    Quaternion selectedRotation;

    [SerializeField] bool isHoverable = false;
    [SerializeField] bool isSelected = false;

    public Action OnCardSelected;
    public Action OnCardDeselected; 

    public void Init(IUISystem uiSystem)
    {
        this.uiSystem = uiSystem;

        cardMovement.OnCardMoved -= OnPRSUpdate; 
        cardMovement.OnCardMoved += OnPRSUpdate;
        // 카드가 움직이고 있을 때는 Hover가 되어서는 안된다. (드로우를 하거나, 카드를 소환, 이동할 때 마우스가 해당 경로에 있으면 HoverSystem이 발동해서는 안된다) 
        cardMovement.OnCardMovedComplete -= OnHoverEnable; 
        cardMovement.OnCardMovedComplete += OnHoverEnable;

        originScale = card.isMyCard ? Vector3.one : Vector3.one * 2; 

        hoverOffset = Vector3.up * 0.01f;
        hoverScale = originScale * 1.1f;

        selectedOffset = Vector3.up * 0.5f + Vector3.forward * 0.5f;
        selectedScale = originScale * 1.3f;
        selectedRotation = card.isMyCard ? Quaternion.Euler(0f, 0f, -180f) : Quaternion.Euler(0f, 180f, 0f);
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

        // 이 부분은 나중에 어떻게 해결할지 정해야할 듯 
        uiSystem.DisplayUI(card);

        Vector3 targetPosition = originPos + selectedOffset;
        Quaternion targetRotation = selectedRotation;
        Vector3 targetScale = selectedScale;

        cardMovement.MoveTransform(new PRS(targetPosition, targetRotation, targetScale), 0.2f, true, () => { OnCardSelected?.Invoke();});
        isSelected = true;

        
    }
    public void OnDeselected()
    {
        uiSystem.CloseUI(); 

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
