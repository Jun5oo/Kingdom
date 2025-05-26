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
}
