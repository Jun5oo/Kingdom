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
                {
                    // 만약 highlight된 위치에 적 카드가 존재한다면, 해당 Cell에 이벤트 등록 (이 Cell들을 클릭하면 공격이 실행) 
                    if (gridSystem.IsObjectOnGridPosition(availablePosition))
                    {
                        if(!gridSystem.GetGameObjectOnGrid(availablePosition).GetComponent<Card>().IsMyCard)
                            gridSystem.OnActionOccured += Attack;
                    }

                    return true; 
                }
            }

            return false; 
        });

    }

    public void Exit()
    {
        // 기존에 구독되어있던 이벤트를 해제 
        gridSystem.OnActionOccured -= Attack;
        // 모든 GridCell을 원래 상태로 되돌리기 
        gridSystem?.UnhighlightGridCells(); 
    }

    // TODO 
    // Attack이 제대로 구현되어있지 않음. 
    // 공격 애니메이션, 데미지 처리 필요. 
    public void Attack(Vector2Int gridPosition)
    {
        Exit();
        
        Vector3 worldPos = gridSystem.GetWorldPosition(gridPosition);
        GameObject obj = GameObject.Instantiate(particleObject, worldPos, Quaternion.identity); 
        obj.GetComponent<ParticleSystem>().Play();

        actionSystem?.CancelAction(); 
    }

    public bool IsValid()
    {
        // 카드가 필드에 위치한 것이 아니라면 공격 불가능 
        if (card.CardState == CardState.Field)
            return true; 

        return false;
    }
}
