using UnityEngine;

public class Card : MonoBehaviour
{
    CardData cardData;
    [SerializeField] CardState cardState;
    
    public bool isMyCard;

    [SerializeField] CardHover cardHover;
    [SerializeField] CardMovement cardMovement;
    [SerializeField] CardActionController cardActionController;
    [SerializeField] private bool isKing; 

    public bool IsKing => isKing;

    public CardState CardState 
    {   get { return cardState; }
        set { cardState = value; }
    }

    public void Init(IUISystem uiSystem, IGridSystem gridSystem, IActionSystem actionSystem, bool isMyCard, CardData cardData = null)
    {
        this.cardData = cardData;
        this.cardState = CardState.Hand;

        this.isMyCard = isMyCard; 

        cardHover?.Init(uiSystem);
        cardActionController?.Init(uiSystem, actionSystem, gridSystem); 
    }
}
