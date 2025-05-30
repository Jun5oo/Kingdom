using System.Collections.Generic;
using UnityEngine;

public class SummonAction : IAction
{
    ActionType actionType; 
    public ActionType ActionType { get { return actionType; } }

    IGridSystem gridSystem;
    IActionSystem actionSystem; 

    GameObject card; 

    public SummonAction(IGridSystem gridSystem, IActionSystem actionSystem, GameObject card)
    {
        actionType = ActionType.Summon;

        // 현재는 GameObject를 받지만, 추후에는 CardData를 받을 것 
        this.card = card;
        this.gridSystem = gridSystem; 
        this.actionSystem = actionSystem;
    }

    public void Enter()
    {
        Exit();
        // 왕의 위치 찾기
        GameObject king = GameObject.FindWithTag("King");
        if (king == null)
        {
            Debug.LogWarning("왕이 배치되지 않았습니다!");
            return;
        }

        Vector2Int kingPos = gridSystem.GetGridPosition(king.transform.position);
        List<Vector2Int> validPositions = GetAdjacentPositions(kingPos);

        gridSystem.HighlightGridCells((Vector2Int gridPosition) =>
        {
            return validPositions.Contains(gridPosition) && !gridSystem.IsObjectOnGridPosition(gridPosition);
        });

        gridSystem.OnActionOccured += SummonCard;
    }

    public void Exit()
    {
        gridSystem.UnhighlightGridCells();
        gridSystem.OnActionOccured -= SummonCard;
    }

    public bool IsValid()
    {
        if (card.GetComponent<Card>().CardState == CardState.Field)
            return false; 

        return true; 
    }

    public void SummonCard(Vector2Int gridPosition)
    {
        Exit();

        Vector3 targetPos = gridSystem.GetWorldPosition(gridPosition) + (Vector3.up * 0.2f);
        PRS prs = new PRS(targetPos, Quaternion.identity, Vector3.one);

        CardMovement cardMovement = card.GetComponent<CardMovement>();
        cardMovement.MoveTransform(prs, 0.5f, false, ()=> {
           
            gridSystem.PlaceObjectTo(card, gridPosition);
            actionSystem?.CancelAction(); 
        });

        // 나중에 event를 통해서 변경하는 방법 모색 
        Card _card = card.GetComponent<Card>();
        _card.CardState = CardState.Field;
    }

    private List<Vector2Int> GetAdjacentPositions(Vector2Int center)
    {
        List<Vector2Int> positions = new List<Vector2Int>();
        Vector2Int[] deltas = {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right,
            new Vector2Int(-1, 1),  // ↖
            new Vector2Int(1, 1),   // ↗
            new Vector2Int(-1, -1), // ↙
            new Vector2Int(1, -1)   // ↘
        };

        foreach (var delta in deltas)
        {
            Vector2Int checkPos = center + delta;
            if (gridSystem.IsValidPosition(checkPos))
                positions.Add(checkPos);
        }

        return positions;
    }
}
