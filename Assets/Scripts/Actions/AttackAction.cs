using System.Collections.Generic;
using UnityEngine;

public class AttackAction : IAction
{
    ActionType actionType = ActionType.Attack; 
    public ActionType ActionType { get { return actionType; } }

    IGridSystem gridSystem;
    IActionSystem actionSystem; 

    GameObject card;

    List<Vector2Int> positions;

    GameObject particleObject; 

    public AttackAction(IGridSystem gridSystem, IActionSystem actionSystem, GameObject card)
    {
        this.gridSystem = gridSystem;
        this.actionSystem = actionSystem;

        this.card = card;
    
        positions = new List<Vector2Int>();
        positions.Add(new Vector2Int(-1, 1));
        positions.Add(new Vector2Int(0, 1));
        positions.Add(new Vector2Int(1, 1));
        positions.Add(new Vector2Int(-1, 0));
        positions.Add(new Vector2Int(1, 0));
        positions.Add(new Vector2Int(-1, -1));
        positions.Add(new Vector2Int(0, -1));
        positions.Add(new Vector2Int(1, -1));

        particleObject = Resources.Load<GameObject>("Particle");
    }

    public void Enter()
    {
        gridSystem.HighlightGridCells((Vector2Int gridPosition) =>
        {
            Vector2Int currentPosition = gridSystem.GetGridPositionOfGameObject(card);
            
            foreach(Vector2Int position in positions)
            {
                Vector2Int availablePosition = currentPosition + position;
                if (gridPosition == availablePosition)
                {
                    if (gridSystem.IsObjectOnGridPosition(availablePosition))
                    {
                        if(!gridSystem.GetGameObjectOnGrid(availablePosition).GetComponent<Card>().isMyCard)
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
        gridSystem.OnActionOccured -= Attack;
        gridSystem.UnhighlightGridCells(); 
    }

    public void Attack(Vector2Int gridPosition)
    {
        Exit();
        
        Debug.Log("Attack Object on gridPosition");
        Vector3 worldPos = gridSystem.GetWorldPosition(gridPosition);
        GameObject obj = GameObject.Instantiate(particleObject, worldPos, Quaternion.identity); 
        obj.GetComponent<ParticleSystem>().Play();

        actionSystem?.CancelAction(); 

    }

    public bool IsValid()
    {
        if (card.GetComponent<Card>().CardState != CardState.Hand)
            return true; 
        return false;
    }
}
