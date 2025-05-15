using UnityEngine;

public class SummonTest : IAction
{
    GridSystem gridSystem;  
    [SerializeField] GameObject cardPrefab;

    public SummonTest(GridSystem gridSystem, GameObject cardPrefab)
    {
        this.gridSystem = gridSystem;
        this.cardPrefab = cardPrefab;
    }

    public void Enter()
    {
        Exit();

        gridSystem.HighlightGridCells((Vector2Int gridPosition) =>
        {
            if (gridSystem.IsObjectOnGridPosition(gridPosition))
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
        return true; 
    }

    public void SummonCard(Vector2Int gridPosition)
    {
        Exit();
        
        Transform spawnPos = GameObject.Find("Hand").transform;
        GameObject gameObject = GameObject.Instantiate(cardPrefab, spawnPos.position, Quaternion.identity);

        gridSystem.PlaceObjectTo(gameObject, gridPosition);
        Vector3 targetPos = gridSystem.GetWorldPosition(gridPosition); 

        CardMovement cardMovement = gameObject.GetComponent<CardMovement>();
        PRS prs = new PRS(targetPos, Quaternion.identity, Vector3.one);
        cardMovement.MoveTransform(prs, 0.5f); 
    }
}
