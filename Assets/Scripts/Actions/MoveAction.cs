using System.Collections.Generic;
using UnityEngine;
public class MoveAction : IAction
{
    IGridSystem gridSystem;
    IActionSystem actionSystem; 

    GameObject obj;

    List<Vector2Int> positions;

    ActionType actionType;
    public ActionType ActionType { get { return actionType; } }

    public MoveAction(IGridSystem gridSystem, IActionSystem actionSystem, GameObject obj)
    {
        // CardData�� ��� Data���� �̵��Ÿ��� �����´�, ������ ��� 1�� �׽�Ʈ�� �����Ѵ�. 
        // ����� �켱 obj�� �޾ƿ���, �̵������� Cell�� ����  

        this.gridSystem = gridSystem;
        this.actionSystem = actionSystem;

        this.obj = obj; 

        positions = new List<Vector2Int>();

        positions.Add(new Vector2Int(-1, 0));
        positions.Add(new Vector2Int(1, 0));
        positions.Add(new Vector2Int(0, 1));
        positions.Add(new Vector2Int(0, -1));

        actionType = ActionType.Move;
    }

    public void Enter()
    {
        Exit();

        gridSystem.HighlightGridCells((Vector2Int gridPosition) =>
        {
            Vector2Int currentGridPosition = gridSystem.GetGridPositionOfGameObject(obj); 

            foreach(Vector2Int position in positions)
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
        if (obj.GetComponent<Card>().CardState != CardState.Hand)
            return true;

        return false; 
    }

    public void MoveToCell(Vector2Int gridPosition)
    {
        Exit();

        Vector2Int currentPos = gridSystem.GetGridPositionOfGameObject(obj);
        Vector3 targetPos = gridSystem.GetWorldPosition(gridPosition);
        PRS prs = new PRS(targetPos, obj.transform.rotation, Vector3.one); 

        CardMovement cardMovement = obj.GetComponent<CardMovement>();
        cardMovement.MoveTransform(prs, 0.7f, false, () => 
        { 
            gridSystem.MoveObjectFrom(currentPos, gridPosition);
            actionSystem?.CancelAction(); 
        });
    }

}
