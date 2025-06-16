using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 공격액션 클래스 
/// </summary>

public class AttackAction : IAction
{
    ActionType actionType; 
    public ActionType ActionType { get { return actionType; } }

    IGridSystem gridSystem;
    IActionSystem actionSystem; 

    Card card;

    List<Vector2Int> positions;

    GameObject particleObject; 

    public AttackAction(IGridSystem gridSystem, IActionSystem actionSystem, Card card)
    {
        // 공격 액션 초기화 
        actionType = ActionType.Attack; 

        this.gridSystem = gridSystem;
        this.actionSystem = actionSystem;

        this.card = card;

        // 카드데이터로부터 공격범위 가져옴 
        positions = card.cardData.attackRange; 

        // Temp 공격효과 
        particleObject = Resources.Load<GameObject>("Particle");
        // 
    }

    public void Enter()
    {
        gridSystem.OnActionOccured -= Attack;
        gridSystem.OnActionOccured += Attack;

        // gridPosition: gridSystem에서 검증할 gridCell의 position 
        gridSystem?.HighlightGridCells((Vector2Int gridPosition) =>
        {
            // 현재 카드의 위치 
            Vector2Int currentPosition = gridSystem.GetGridPositionOfGameObject(card.gameObject);
       
            foreach(Vector2Int position in positions)
            {
                // 현재 카드위치에서 공격범위를 더해 해당 값이 gridPosition과 같다면 highlight 
                Vector2Int availablePosition = currentPosition + position;
                if (gridPosition == availablePosition)
                    return true; 
            }

            return false; 
        }, HighlightType.ValidAttack, HighlightLayer.Action);

    }

    public void Exit()
    {
        // 기존에 구독되어있던 이벤트를 해제 
        gridSystem.OnActionOccured -= Attack;
        // 모든 GridCell을 원래 상태로 되돌리기 
        gridSystem?.UnhighlightGridCells(HighlightLayer.Action);
        gridSystem?.UnhighlightGridCells(HighlightLayer.Hover); 
    }

    public void Attack(Vector2Int gridPosition)
    {
        Exit();

        GameObject obj = gridSystem?.GetGameObjectOnGrid(gridPosition);

        if (obj == null)
        {
            actionSystem?.CancelAction();
            return; 
        }

        if (obj != null)
        {
            if (obj.TryGetComponent<IDamageable>(out IDamageable damageable))
            {
                if (damageable.IsAlies())
                    return; 

                Vector3 position = gridSystem.GetWorldPosition(gridPosition);
                PRS cardPRS = card.GetComponent<CardMovement>().PRS;

                card.GetComponent<CardMovement>().AttackTargetFrom(
                    position,
                    cardPRS,
                    onHitCallback: () =>
                    {
                        Card onHitCard = obj.GetComponent<Card>();

                        int counterDamage = onHitCard.CP; 

                        damageable?.TakeDamage(card.CP);
                        Vector2Int attackerPos = gridSystem.GetGridPositionOfGameObject(card.gameObject);
                        
                        foreach(Vector2Int pos in onHitCard.AttackRange)
                        {
                            if(gridPosition + pos == attackerPos)
                            {
                                card?.GetComponent<IDamageable>().TakeDamage(counterDamage);
                                break; 
                            }
                        }

                    },
                    onCompleteCallback: () =>
                    {
                        actionSystem?.CancelAction();
                        GameObject.FindAnyObjectByType<GameFlowManager>()?.OnActionPerformed();
                    }
                );
            }
        }
    }

    public bool IsValid()
    {
        // 카드가 필드에 위치한 것이 아니라면 공격 불가능 
        if (card.CardState == CardState.Field)
            return true; 

        return false;
    }
}
