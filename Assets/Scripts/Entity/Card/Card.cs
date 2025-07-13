using System.Collections.Generic;
using UnityEngine;

public class Card : Entity
{
    [Header("Original Data")]
    public CardData cardData;

    [Header("RunTime Data")]
    [SerializeField] private string cardName;
    [SerializeField] private Sprite cardSprite; 
    [SerializeField] private string cardDescription;
    [SerializeField] private int level;
    [SerializeField] private int currentCP;
    [SerializeField] private int movement;
    [SerializeField] private bool isKing;
    [SerializeField] private List<ActionType> actionTypes;
    [SerializeField] private List<Vector2Int> attackRange;
    [SerializeField] private List<Vector2Int> moveRange; 
    [SerializeField] private int ownerPlayerID; 

    [Header("Components")]
    [SerializeField] CardView cardView; 
    [SerializeField] CardHover cardHover;
    [SerializeField] CardMovement cardMovement;

    public override string Name { get { return cardName; } }
    public override Sprite Sprite { get { return cardSprite; } }
    public override string Description { get {  return cardDescription; } }
    public override int Level { get { return level; } }
    public override int CP { get {  return currentCP; } }
    public override int Movement { get { return movement; } }
    public bool IsKing { get { return isKing; } }
    public override List<ActionType> Actions { get { return actionTypes; } }
    public List<Vector2Int> MoveableRange { get { return moveRange; } }
    public List<Vector2Int> AttackRange { get { return attackRange; } }
    public override int OwnerPlayerID { get { return ownerPlayerID; } }

    public void Init(CardData cardData, int playerID)
    {
        // 카드가 생성될 때 초기화 
        this.cardData = cardData;

        this.cardName = cardData.Name;
        this.cardSprite = cardData.Sprite;
        this.level = cardData.Level;
        this.cardDescription = cardData.Description; 
        this.currentCP = cardData.CP;
        this.movement = cardData.Movement;
        this.isKing = cardData.IsKing;
        this.actionTypes = cardData.Actions;
        this.attackRange = cardData.AttackRange;
        this.moveRange = cardData.MoveRange;
        this.ownerPlayerID = playerID;

        cardView.Init(Sprite); 
        cardHover?.Init();
    }
}
