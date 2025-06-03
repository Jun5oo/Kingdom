using System.Collections.Generic;
using UnityEngine;
public class MoveAction : IAction
{
    ActionType actionType;
    public ActionType ActionType { get { return actionType; } }

    IGridSystem gridSystem;
    IActionSystem actionSystem;

    Card card; 

    List<Vector2Int> moveablePositions;

    public MoveAction(IGridSystem gridSystem, IActionSystem actionSystem, Card card)
    {
        actionType = ActionType.Move;

        this.gridSystem = gridSystem;
        this.actionSystem = actionSystem;

        this.card = card;
        this.moveablePositions = card.cardData.moveRange; 
    }

    public void Enter()
    {
        Exit();

        gridSystem.HighlightGridCells((Vector2Int gridPosition) =>
        {
            Vector2Int currentGridPosition = gridSystem.GetGridPositionOfGameObject(card.gameObject); 

            foreach(Vector2Int position in moveablePositions)
            {
                Vector2Int availablePosition = currentGridPosition + position; 
                if (availablePosition == gridPosition && !gridSystem.IsObjectOnGridPosition(gridPosition))
                    return true; 
            }

            return false; 
        });

        gridSystem.OnActionOccured += MoveToCell; 
    }
    public void Exit()
    {
        gridSystem.UnhighlightGridCells(); 
        gridSystem.OnActionOccured -= MoveToCell; 
    }
    public bool IsValid()
    {
        if (card.CardState != CardState.Hand)
            return true;

        return false; 
    }

    public void MoveToCell(Vector2Int gridPosition)
    {
        Exit();

        // Temp 
        card.GetComponent<CardView>().HideStatusUI(); 
        //

        Vector2Int currentPos = gridSystem.GetGridPositionOfGameObject(card.gameObject);

        Vector3 targetPos = gridSystem.GetWorldPosition(gridPosition);
        PRS prs = new PRS(targetPos, card.gameObject.transform.rotation, Vector3.one); 

        CardMovement cardMovement = card.gameObject.GetComponent<CardMovement>();
        cardMovement.MoveTransform(prs, 0.7f, false, () => 
        { 
            gridSystem.MoveObjectFrom(currentPos, gridPosition);
            actionSystem?.CancelAction(); 
            card.GetComponent<CardView>().DisplayStatusUI();
            GameObject.FindAnyObjectByType<GameFlowManager>()?.OnActionPerformed();
        });
    }

}
