using UnityEngine;

public class GridCellHandler
{
    IActionSystem actionSystem;
    IGridSystem gridSystem; 

    public GridCellHandler(IActionSystem actionSystem, IGridSystem gridSystem)
    {
        this.gridSystem = gridSystem;
        this.actionSystem = actionSystem;
    }

    public void OnGridHovered(GridCell gridCell)
    {
        if (!actionSystem.IsActionInProgress())
            return;

        Vector2Int gridPosition = gridCell.GetGridPosition();
        
        if (!gridSystem.IsObjectOnGridPosition(gridPosition))
            return;

        Card card = gridSystem.GetGameObjectOnGrid(gridPosition).GetComponent<Card>();

        gridSystem.HighlightGridCells((Vector2Int gridCellPosition) =>
        {
            foreach(Vector2Int validPosition in card.AttackRange)
            {
                if (gridCellPosition == validPosition + gridPosition)
                    return true; 
            }

            return false; 
        }, HighlightType.EnemyAttackRange, HighlightLayer.Hover);
    }

    public void OnGridUnHovered()
    {
        if (!actionSystem.IsActionInProgress())
            return; 

        gridSystem.UnhighlightGridCells(HighlightLayer.Hover); 
    }
}
