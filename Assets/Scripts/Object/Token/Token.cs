using System;
using System.Collections.Generic;
using UnityEngine;

public enum TokenState
{
    Alive = 0, 
    Graveyard = 1, 
    Dead = 2
}

public class Token : BaseObject, IDamageable
{
    [Header("RunTime Data")]
    [SerializeField] int currentCP;
    [SerializeField] int ownerID;

    [SerializeField] TokenMovement movement;
    [SerializeField] TokenInteraction interaction;
    [SerializeField] TokenView view;

    TokenState tokenState;
    IDeathBehaviour deathBehaviour;

    public UnitCardData UnitData { get { return Data as UnitCardData; } }
    public int CP { get { return currentCP; } }
    public int MAXCP { get { return UnitData.CP; } }
    public int Movement { get { return UnitData.Movement; } }
    public bool IsKing { get { return UnitData.IsKing; } }
    public override int OwnerID { get { return ownerID; } }

    public List<Vector2Int> MoveableRange { get { return UnitData.MoveRange; } }
    public List<Vector2Int> AttackRange { get { return UnitData.AttackRange; } }
    
    public TokenState TokenState {  get { return tokenState; } }
    public IDeathBehaviour DeathBehaviour { get {  return deathBehaviour; } }

    public void Init(UnitCardData unitData, int playerID)
    {
        base.Init(unitData); 

        this.currentCP = MAXCP;
        this.ownerID = playerID;

        this.tokenState = TokenState.Alive; 

        movement.Init();
        interaction.Init(this); 

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
        return OwnerID == playerID; 
    }

    public Action<int> OnCPUpdate; 
    public void TakeDamage(int damage, bool isDirect = false)
    {
        if (isDirect && IsKing)
            damage *= 2; 

        currentCP -= damage;
        view.OnUpdateCP(currentCP);
        OnCPUpdate?.Invoke(currentCP);
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
        this.currentCP = cp;

        view?.OnUpdateCP(currentCP);
        view?.OnUpdateMovement(movement); 
    }
    public void Revive()
    {
        if (TokenState != TokenState.Graveyard)
            return;

        tokenState = TokenState.Alive;
        SetTokenStatus(MAXCP, Movement, MoveableRange, AttackRange);
    }
    void OnDestroy()
    {
        OnCPUpdate = null; 
    }
}
