using UnityEngine;

/// <summary>
/// 소환액션 클래스
/// </summary>

public class SummonAction : IAction
{
    private ActionType actionType; 
    public ActionType ActionType { get { return actionType; } }

    IGridSystem gridSystem;
    IActionSystem actionSystem;

    Card card; 

    public SummonAction(IGridSystem gridSystem, IActionSystem actionSystem, Card card)
    {
        actionType = ActionType.Summon;

        this.gridSystem = gridSystem; 
        this.actionSystem = actionSystem;
        this.card = card;
    }

    public void Enter()
    {
        Exit();

        gridSystem.HighlightGridCells((Vector2Int gridPosition) =>
        {
            return CanSummonAt(gridPosition); 
        });

        gridSystem.OnActionOccured += SummonCard;
    }

    public void Exit()
    {
        gridSystem?.UnhighlightGridCells();
        gridSystem.OnActionOccured -= SummonCard;
    }

    public bool IsValid()
    {
        // 추후에는 다른 플레이어의 왕 카드 또는 카드는 소환할 수 없어야 함 
        // if(!card.IsMyCard) return false; 

        return card.CardState == CardState.Hand; 
    }

    public void SummonCard(Vector2Int gridPosition)
    {
        Exit();
        // Temp 
        CardSystem cardSystem = GameObject.FindAnyObjectByType<CardSystem>();
        if (card.IsMyCard)
            cardSystem.RemoveCardFromHand(0, card);
        else
            cardSystem.RemoveCardFromHand(1, card); 

        // 수치가 하드코딩됨 나중에 
        Vector3 targetPos = gridSystem.GetWorldPosition(gridPosition) + (Vector3.up * 0.2f);
        Vector3 eulerAngles = card.IsMyCard ? new Vector3(0f, 0f, 180f) : new Vector3(0f, 180f, 180f);
        Quaternion quaternion = Quaternion.Euler(eulerAngles); 
        PRS prs = new PRS(targetPos, quaternion, Vector3.one);

        CardMovement cardMovement = card.GetComponent<CardMovement>();
        cardMovement.MoveTransform(prs, 0.5f, false, ()=> {
            gridSystem.PlaceObjectTo(card.gameObject, gridPosition);
            actionSystem?.CancelAction();
            // Temp 
            card.GetComponent<CardView>().DisplayStatusUI();
            // 
            GameObject.FindAnyObjectByType<GameFlowManager>()?.OnActionPerformed();
        });

        // 나중에 event를 통해서 변경하는 방법 모색 
        card.CardState = CardState.Field;
    }

    private bool CanSummonAt(Vector2Int pos)
    {
        if (gridSystem.IsObjectOnGridPosition(pos))
            return false;

        if (card.IsKing)
        {
            if (card.IsMyCard)
                return pos.y < 3;
            else
                return pos.y >= 5; 
        }
    
        Vector2Int center = GetKingPosition();
        // 추후 GetKingPosition 다시 작성 

        if (center == -Vector2Int.one)
            return false;

        int distanceX = Mathf.Abs(center.x - pos.x);
        int distanceY = Mathf.Abs(center.y - pos.y); 

        return distanceX <= 1 && distanceY <= 1;
    }

    private Vector2Int GetKingPosition()
    {
        GameObject[] kings = GameObject.FindGameObjectsWithTag("King");

        foreach (GameObject kingObj in kings)
        {
            Card kingCard = kingObj.GetComponent<Card>();
            if (kingCard != null && kingCard.IsMyCard == card.IsMyCard)
            {
                return gridSystem.GetGridPositionOfGameObject(kingObj);
            }
        }

        return -Vector2Int.one;
    }
}
