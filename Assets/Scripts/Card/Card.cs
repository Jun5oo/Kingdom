using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 카드 데이터 컨테이너 클래스
/// </summary>

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
    [SerializeField] private List<ActionType> actionTypes;

    // Temp, CardDisplay UI를 나타내기 위한 데이터 
    [SerializeField] private int movement;

    [Header("Card State")]
    [SerializeField] CardState cardState;

    private bool isMyCard;
    private bool isKing;
    public bool IsKing => isKing;
    public bool IsMyCard => isMyCard; 

    [Header("Components")]
    [SerializeField] CardView cardView; 
    [SerializeField] CardHover cardHover;
    [SerializeField] CardMovement cardMovement;
    [SerializeField] CardActionController cardActionController;


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
    public List<ActionType> Actions { get { return actionTypes; } }

    public void Init(IUISystem uiSystem, IGridSystem gridSystem, IActionSystem actionSystem, bool isMyCard, CardData cardData)
    {
        // 카드가 생성될 때 초기화 
        this.cardData = cardData;

        this.cardName = cardData.cardName;
        this.cardImage = cardData.sprite; 
        this.cardDescription = cardData.description;
        this.level = cardData.level;
        this.currentCp = cardData.cp;

        this.movement = cardData.movement;
        this.actionTypes = cardData.actions; 

        this.cardState = CardState.Hand;
        this.isMyCard = isMyCard;
        this.isKing = cardData.isKing;

        if (IsKing)
            this.gameObject.tag = "King"; 
        
        cardView?.Init(this); 
        cardHover?.Init(uiSystem);
        cardActionController?.Init(uiSystem, actionSystem, gridSystem); 
    }
}
