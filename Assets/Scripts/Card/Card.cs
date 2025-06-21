using System;
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
    [SerializeField] private int currentCP;

    [SerializeField] private List<Vector2Int> attackRange; 
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
    [SerializeField] CardDamageController cardDamageController;

    public CardState CardState 
    {   get { return cardState; }
        set { cardState = value; }
    }

    public string Name { get { return cardName; } }
    public Sprite Image { get { return cardImage; } }
    public string Description { get {  return cardDescription; } }
    public int Level { get { return level; } }
    public int CP { get {  return currentCP; } private set { currentCP = value; } }

    public int Movement { get { return movement; } }

    public List<Vector2Int> AttackRange { get { return attackRange; } }
    public List<ActionType> Actions { get { return actionTypes; } }

    public void Init(IUISystem uiSystem, IGridSystem gridSystem, IActionSystem actionSystem, bool isMyCard, CardData cardData)
    {
        // 카드가 생성될 때 초기화 
        this.cardData = cardData;

        this.cardName = cardData.cardName;
        this.cardImage = cardData.sprite; 
        this.cardDescription = cardData.description;
        this.level = cardData.level;
        this.currentCP = cardData.cp;

        this.attackRange = cardData.attackRange;
        this.actionTypes = cardData.actions;
        this.movement = cardData.movement;

        this.cardState = CardState.Hand;
        this.isMyCard = isMyCard;
        this.isKing = cardData.isKing;

        if (IsKing)
            this.gameObject.tag = "King"; 
        
        cardHover?.Init(uiSystem);
        cardView?.Init(uiSystem, this);
        cardDamageController?.Init(uiSystem, this);
        cardActionController?.Init(uiSystem, actionSystem, gridSystem);

        cardDamageController.OnDamaged -= OnCPUpdate; 
        cardDamageController.OnDamaged += OnCPUpdate;
    }

    public Action<int, int> OnCPChanged; 

    public void OnCPUpdate(int damage)
    {
        CP -= damage; 

        cardView.UpdateStatusUI();

        int playerID = isMyCard ? 0 : 1; 
        OnCPChanged?.Invoke(playerID, CP);

        if (CP <= 0)
        {
            //Temp 
            GridSystem gridSystem = FindAnyObjectByType<GridSystem>();
            Vector2Int gridPosition = gridSystem.GetGridPositionOfGameObject(this.gameObject);
            gridSystem.RemoveObjectFrom(this.gameObject, gridPosition);

            Destroy(this.gameObject);
        }   
    }

    public void OnDestroy()
    {
        cardDamageController.OnDamaged -= OnCPUpdate; 
    }
}
