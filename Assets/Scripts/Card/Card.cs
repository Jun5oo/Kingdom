using UnityEngine;
using UnityEngine.Rendering;

public class Card : MonoBehaviour
{
    [Header("Original Data")]
    public CardData cardData;

    [Header("RunTime Data")]
    [SerializeField] private string cardName;
    [SerializeField] private Sprite cardImage; 
    [SerializeField] private string cardDescription;
    [SerializeField] private int level;
    [SerializeField] private int currentCp;
    // Temp 
    [SerializeField] private int movement; 
    // 

    [Header("Card State")]
    [SerializeField] CardState cardState;

    public bool isMyCard;

    [Header("Components")]
    [SerializeField] CardView cardView; 
    [SerializeField] CardHover cardHover;
    [SerializeField] CardMovement cardMovement;
    [SerializeField] CardActionController cardActionController;
    [SerializeField] private bool isKing; 

    public bool IsKing => isKing;

    public CardState CardState 
    {   get { return cardState; }
        set { cardState = value; }
    }

    public string Name { get { return cardName; } }
    public Sprite Image { get { return cardImage; } }
    public string Description { get {  return cardDescription; } }
    public int Level { get { return level; } }
    public int Cp { get {  return currentCp; } }

    public int Movement { get { return movement; } }

    public void Init(IUISystem uiSystem, IGridSystem gridSystem, IActionSystem actionSystem, bool isMyCard, CardData cardData)
    {
        this.cardData = cardData;

        this.cardName = cardData.cardName;
        this.cardImage = cardData.sprite; 
        this.cardDescription = cardData.description;
        this.level = cardData.level;
        this.currentCp = cardData.cp;
        this.movement = cardData.movement; 

        this.cardState = CardState.Hand;
        this.isMyCard = isMyCard;

        if(cardData != null) this.isKing = cardData.isKing;
        cardView?.Init(this); 
        cardHover?.Init(uiSystem);
        cardActionController?.Init(uiSystem, actionSystem, gridSystem); 
    }
}
