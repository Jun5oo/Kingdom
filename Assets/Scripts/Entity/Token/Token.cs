using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Token : Entity, IDamageable
{
    [SerializeField] CardData cardData; 

    [SerializeField] string tokenName;
    [SerializeField] Sprite tokenSprite;
    [SerializeField] string tokenDescription;
    [SerializeField] List<ActionType> actions; 

    [SerializeField] int tokenLevel;
    [SerializeField] int currentTokenCP;
    [SerializeField] int currentTokenMovement;

    [SerializeField] int ownerPlayerID; 

    [SerializeField] bool isKing; 

    [SerializeField] List<Vector2Int> attackRange;
    [SerializeField] List<Vector2Int> moveRange;

    [SerializeField] TokenMovement tokenMovement;
    [SerializeField] TokenHover tokenHover; 
    [SerializeField] TokenView tokenView;

    public override string Name { get { return tokenName; } } 
    public override Sprite Sprite { get { return tokenSprite;} }
    public override string Description { get { return tokenDescription; } }
    public override List<ActionType> Actions { get { return actions; } }
    public override int Level { get { return tokenLevel; } } 
    public override int CP { get { return currentTokenCP; } }
    public override int Movement { get { return currentTokenMovement; } }
    public bool IsKing { get { return isKing; } }

    public List<Vector2Int> MoveRange {  get { return moveRange; } }
    public List<Vector2Int> AttackRange { get { return attackRange; } }

    public override int OwnerPlayerID { get { return ownerPlayerID; } }

    public void Init(CardData cardData, int playerID)
    {
        this.cardData = cardData;

        this.tokenName = cardData.Name; 
        this.tokenSprite = cardData.Sprite;
        this.tokenLevel = cardData.Level;
        this.tokenDescription = cardData.Description;
        this.currentTokenCP = cardData.CP;
        this.currentTokenMovement = cardData.Movement;
        this.isKing = cardData.IsKing;
        this.actions = cardData.Actions;
        this.moveRange = cardData.MoveRange;
        this.attackRange = cardData.AttackRange;

        this.ownerPlayerID = playerID; 

        tokenMovement.Init();
        tokenHover.Init(); 
        tokenView.Init(Sprite, CP, Movement);
    }
    public bool IsAllies(Token token)
    {
        return OwnerPlayerID == token.ownerPlayerID; 
    }

    public Action<int> OnCPUpdate; 

    public void TakeDamage(int damage, bool isDirect = false)
    {
        if (isDirect && IsKing)
            damage *= 2; 

        currentTokenCP -= damage;
        tokenView.OnUpdateCP(currentTokenCP);
        OnCPUpdate?.Invoke(currentTokenCP);
    }

    void OnDestroy()
    {
        // HUD와의 연결해제 필요. 
        OnCPUpdate = null; 
    }
}
