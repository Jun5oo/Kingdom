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
    public int Movement { get { return unitData.Movement; } }
    public Race Race { get { return unitData.Race; } }
    public bool IsKing { get { return unitData.IsKing; } }
    public List<Vector2Int> MoveRange { get { return unitData.MoveRange; } }
    public List<Vector2Int> AttackRange { get { return unitData.AttackRange; } }
    public TokenState TokenState {  get { return tokenState; } }
    public IDeathBehaviour DeathBehaviour { get {  return deathBehaviour; } }

    public void Init(UnitCardData unitData, int playerID)
    {
        this.unitData = unitData; 
        this.currentTokenCP = unitData.CP;
        this.ownerPlayerID = playerID;

        this.tokenState = TokenState.Alive; 

        tokenMovement.Init();
        tokenHover.Init(); 
        tokenView.Init(Sprite, CP, Movement);

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
    void OnDestroy()
    {
        OnCPUpdate = null; 
    }
}
