using System;
using System.Collections.Generic;
using UnityEngine;

public enum TokenState
{
    Alive = 0, 
    Graveyard = 1, 
    Dead = 2
}

public class Token : Entity, IUnit, IDamageable
{
    [SerializeField] UnitCardData unitData;

    [SerializeField] int currentTokenCP;
    [SerializeField] int movement;
    [SerializeField] List<Vector2Int> attackRange;
    [SerializeField] List<Vector2Int> movementRange; 
    [SerializeField] int ownerPlayerID;

    [SerializeField] TokenMovement tokenMovement;
    [SerializeField] TokenHover tokenHover;
    [SerializeField] TokenView tokenView;

    TokenState tokenState;
    IDeathBehaviour deathBehaviour;

    public override string Name { get { return unitData.Name; } }
    public override Sprite Sprite { get { return unitData.Sprite; } }
    public override string Description { get { return unitData.Description; } }
    public override int OwnerPlayerID { get { return ownerPlayerID; } }
    public override List<ActionType> Actions { get { return unitData.Actions; } }

    public int CP { get { return currentTokenCP; } }
    public int MAXCP { get { return unitData.CP; } }
    public int Movement { get { return unitData.Movement; } }
    public int CurrentMovement { get { return movement; } }
    public Texture2D Texture { get { return unitData.CardArt; } }
    public Race Race { get { return unitData.Race; } }
    public bool IsKing { get { return unitData.IsKing; } }
    public List<Vector2Int> MoveRange { get { return unitData.MoveRange; } }
    public List<Vector2Int> AttackRange { get { return unitData.AttackRange; } }
    public List<Vector2Int> CurrentMoveRange { get { return movementRange; } }
    public List<Vector2Int> CurrentAttackRange { get { return attackRange; } }
    public TokenState TokenState {  get { return tokenState; } }
    public IDeathBehaviour DeathBehaviour { get {  return deathBehaviour; } }

    public void Init(UnitCardData unitData, int playerID)
    {
        this.unitData = unitData; 
        this.currentTokenCP = MAXCP;
        this.movement = Movement; 
        this.attackRange = AttackRange;
        this.movementRange = MoveRange; 
        this.ownerPlayerID = playerID;

        this.tokenState = TokenState.Alive; 

        tokenMovement.Init();
        tokenHover.Init(); 
        tokenView.Init(unitData.CardArt, CP, CurrentMovement);

        switch (Race)
        {
            case Race.Undead:
                deathBehaviour = new UndeadDeathBehaviour();
                break;
            default:
                deathBehaviour = new DefaultDeathBehaviour();
                break; 
        }
    }
    public bool IsAllies(int playerID)
    {
        return OwnerPlayerID == playerID; 
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
    public bool TryEnterGraveyard()
    {
        if (TokenState != TokenState.Alive)
            return false; 

        if (Race != Race.Undead || IsKing)
        {
            tokenState = TokenState.Dead; 
            return false;
        }

        tokenState = TokenState.Graveyard;
        return true; 
    }
    public void SetTokenStatus(int cp, int movement, List<Vector2Int> moveRange, List<Vector2Int> attackRange)
    {
        this.currentTokenCP = cp;
        this.movement = movement;
        this.movementRange = moveRange; 
        this.attackRange = attackRange;

        tokenView?.OnUpdateCP(currentTokenCP);
        tokenView?.OnUpdateMovement(movement); 
    }
    public void Revive()
    {
        if (TokenState != TokenState.Graveyard)
            return;

        tokenState = TokenState.Alive;
        tokenView.SetTokenArt(Texture); 
        SetTokenStatus(MAXCP, Movement, MoveRange, AttackRange);
    }
    void OnDestroy()
    {
        OnCPUpdate = null; 
    }
}
