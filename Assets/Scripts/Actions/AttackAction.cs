using System;
using System.Collections.Generic;
using UnityEngine;

public class AttackAction : IAction
{
    ActionType actionType;
    HighlightLayer highlightLayer;
    HighlightType highlightType;

    public ActionType ActionType { get { return actionType; } }
    public HighlightLayer HighlightLayer { get { return highlightLayer; } }
    public HighlightType HighlightType {  get { return highlightType; } }
    public ActionPerformer Performer {  get {  return performer; } }

    GridManager gridManager;
    TokenManager tokenManager;
    DamageManager damageManager;

    Token token;
    Token target;
    ActionPerformer performer;

    Vector2Int targetPosition; 

    List<Vector2Int> attackablePositions;

    public event Action OnActionComplete;
    public event Action OnActionCanceled;

    int currentCost; 
    public int Cost { get { return currentCost; } }

    public AttackAction(Token token, ActionPerformer performer)
    {
        actionType = ActionType.Attack;
        highlightLayer = HighlightLayer.Action;
        highlightType = HighlightType.AttackHighlight;

        this.gridManager = ServiceLocator.Get<GridManager>(); 
        this.damageManager = ServiceLocator.Get<DamageManager>();
        this.tokenManager = ServiceLocator.Get<TokenManager>(); 
        this.token = token;
        this.performer = performer;

        attackablePositions = token.CurrentAttackRange;

        currentCost = 1; 
    }

    public void Enter()
    {
        gridManager.HighlightGridCells((Vector2Int gridPosition) =>
        {
            Vector2Int currentGridPosition = tokenManager.GetGridPositionOfToken(token);

            if (currentGridPosition == -Vector2Int.one)
                return false; 

            foreach (Vector2Int position in attackablePositions)
            {
                Vector2Int availablePosition = currentGridPosition + position;
                if (availablePosition == gridPosition)
                    return true;
            }

            return false;

        }, HighlightType.AttackHighlight, HighlightLayer.Action);
    }
    public void Execute(Vector2Int targetPosition)
    {
        var target = tokenManager.GetTokenFrom(targetPosition);
        this.target = target;
        this.targetPosition = targetPosition; 

        if(target == null)
        {
            Debug.Log("공격할 대상이 존재하지 않습니다!");
            OnActionCanceled?.Invoke(); 
            return; 
        }

        if(!target.TryGetComponent<IDamageable>(out IDamageable damageable))
        {
            Debug.Log("공격할 수 없는 대상입니다! (Non-IDamageable)");
            OnActionCanceled?.Invoke();
            return; 
        }

        if (damageable.IsAllies(token.OwnerPlayerID))
        {
            Debug.Log("아군을 공격할 수 없습니다!");
            OnActionCanceled?.Invoke();
            return; 
        }

        Transition(AttackState.Prepare); 
    }

    public void Exit()
    {
        gridManager?.UnhighlightGridCells(HighlightLayer.Action);
        gridManager?.UnhighlightGridCells(HighlightLayer.Hover); 
    }

    void Transition(AttackState state)
    {
        switch (state)
        {
            case AttackState.Prepare:
                Prepare(); 
                break;
            case AttackState.Animation:
                Attack(); 
                break;
            case AttackState.Placing:
                Placing(); 
                break;
            case AttackState.Done:
                Done(); 
                break;
        }
    }

    void Prepare()
    {
        Exit();
        Transition(AttackState.Animation); 
    }
    
    void Attack()
    {
        Vector3 targetPosition = gridManager.GetWorldPosition(this.targetPosition); 

        TokenMovement tokenMovement = token.GetComponent<TokenMovement>();
        PRS prs = tokenMovement.PRS;

        int counterDamage = target.CP; 

        tokenMovement.AttackTargetFrom(targetPosition, prs, onHitCallback: () =>
        {
            damageManager.ProcessCombat(token, target); 
        },
        onCompleteCallback: () =>
        {
            damageManager.TryProcessCounterAttack(target, token, counterDamage);
            damageManager.TryDestroyToken(token);
            damageManager.TryDestroyToken(target); 
            damageManager.CheckForKingDefeat(); 
            Transition(AttackState.Placing); 
        });
    }

    void Placing()
    {
        Debug.Log("Done"); 
        Transition(AttackState.Done); 
    }

    void Done()
    {
        OnActionComplete?.Invoke(); 
    }

    public bool IsValid()
    {
        if(attackablePositions.Count == 0) 
            return false;

        return ServiceLocator.Get<ActionSystem>().GetCurrentActionCount() >= currentCost;
    }
}
