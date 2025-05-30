using UnityEngine;

public class SummonAction : IAction
{
    ActionType actionType; 
    public ActionType ActionType { get { return actionType; } }

    IGridSystem gridSystem;
    IActionSystem actionSystem;

    Card card; 

    public SummonAction(IGridSystem gridSystem, IActionSystem actionSystem, Card card)
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

        gridSystem.HighlightGridCells((Vector2Int gridPosition) =>
        {
            GridCell gridCell = gridSystem.GetGridCell(gridPosition);

            if (gridSystem.IsObjectOnGridPosition(gridPosition) || !gridCell.isMyCell)
                return false;
            else
                return true;
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
        if (card.GetComponent<Card>().CardState != CardState.Hand)
            return false; 

        return true; 
    }

    public void SummonCard(Vector2Int gridPosition)
    {
        Exit();

        // Temp 
        CardSystem cardSystem = GameObject.FindAnyObjectByType<CardSystem>();
        if (card.isMyCard)
            cardSystem.RemoveCardFromHand(0, card);
        else
            cardSystem.RemoveCardFromHand(1, card); 
        // 

        // 수치가 하드코딩됨 나중에 
        Vector3 targetPos = gridSystem.GetWorldPosition(gridPosition) + (Vector3.up * 0.2f);
        Vector3 eulerAngles = card.isMyCard ? new Vector3(0f, 0f, 180f) : new Vector3(0f, 180f, 180f);
        Quaternion quaternion = Quaternion.Euler(eulerAngles); 
        PRS prs = new PRS(targetPos, quaternion, Vector3.one);

        CardMovement cardMovement = card.GetComponent<CardMovement>();
        cardMovement.MoveTransform(prs, 0.5f, false, ()=> {
            gridSystem.PlaceObjectTo(card.gameObject, gridPosition);
            actionSystem?.CancelAction();
            // Temp 
            card.GetComponent<CardView>().DisplayStatusUI(); 
            // 
        });

        // 나중에 event를 통해서 변경하는 방법 모색 
        card.CardState = CardState.Field;
    }
}
