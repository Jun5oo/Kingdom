using System;
using UnityEngine;

public enum CardState
{
    Idle, 
    Hover, 
    Selected,
    Moving
}

public class CardHover : MonoBehaviour, IHoverable, ISelectable
{
    [Header("Card Components")]
    [SerializeField] Card card;
    [SerializeField] CardMovement cardMovement;

    [Header("PRS")]
    [SerializeField] Vector3 originPos;
    [SerializeField] Quaternion originRotation;
    [SerializeField] Vector3 originScale;

    // OnHover PRS 
    [SerializeField] Vector3 hoverOffset;
    [SerializeField] Vector3 hoverScale;

    // OnSelected PRS 
    Vector3 selectedOffset;
    Vector3 selectedScale;

    [SerializeField] CardState cardState; 

    public Action<Card> OnCardSelected;
    public Action OnCardDeselected;
    public event Action OnSelectedComplete;

    public Entity Entity { get { return card; } }

    public void Init()
    {
        cardState = CardState.Idle; 

        cardMovement.OnCardMoved -= OnUpdatePRS; 
        cardMovement.OnCardMoved += OnUpdatePRS;
        
        cardMovement.OnCardMoveComplete -= OnCardMoveComplete;
        cardMovement.OnCardMoveComplete += OnCardMoveComplete;

        originScale = Vector3.one; 

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

        cardMovement.MoveTransform(new PRS(targetPosition, originRotation, targetScale), 0f, true, () => { cardState = CardState.Hover; });
    }
    public void OffHover()
    {
        if (cardState != CardState.Hover)
            return;

        cardMovement.MoveTransform(new PRS(originPos, originRotation, originScale), 0f, true, () => { cardState = CardState.Idle; }); 
    }
    public bool IsHoverable()
    {
        return cardState == CardState.Idle;
    }
    #endregion

    #region Selection
    public void OnSelected()
    {
        if (!IsSelectable())
            return;

        Vector3 targetPosition = originPos + selectedOffset;
        Quaternion targetRotation = originRotation;
        Vector3 targetScale = selectedScale;

        // 카드가 클릭된 것처럼 보이게 카드의 scale과 높이를 조정, 이후 카드가 선택되었음을 이벤트로 전달 
        cardMovement.MoveTransform(new PRS(targetPosition, targetRotation, targetScale), 0.2f, true, () => 
        {
            cardState = CardState.Selected; 
            OnCardSelected?.Invoke(card);
            OnSelectedComplete?.Invoke(); 
        });

    }
    public void OnDeselected()
    {
        if (cardState != CardState.Selected)
            return;

        cardMovement.MoveTransform(new PRS(originPos, originRotation, originScale), 0.2f, true, ()=>
        {
            cardState = CardState.Idle;
            OnCardDeselected?.Invoke();
        });
    }

    public bool IsSelectable()
    {
        return (cardState == CardState.Idle || cardState == CardState.Hover);
    }
    #endregion

    public void OnUpdatePRS()
    {
        cardState = CardState.Moving; 

        PRS prs = cardMovement.PRS; 

        originPos = prs.position;
        originRotation = prs.rotation;
        originScale = prs.scale;
    }

    void OnCardMoveComplete()
    {
        if(cardState == CardState.Moving)
            cardState = CardState.Idle;
    }

    public void OnDestroy()
    {
        OnCardSelected = null;
        OnCardDeselected = null;

        OnSelectedComplete = null; 

        cardMovement.OnCardMoved -= OnUpdatePRS;
    }
}
