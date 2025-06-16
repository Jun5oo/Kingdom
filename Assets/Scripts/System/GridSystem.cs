using System;
using UnityEngine;

/// <summary>
/// Grid를 관리하는 System 클래스 
/// </summary>

public enum HighlightLayer
{
    Action = 0, 
    Hover = 1, 
    Outline = 2 
}

public enum HighlightType
{
    ValidSummon, 
    ValidMove, 
    ValidAttack, 
    EnemyAttackRange
}

public class GridSystem : MonoBehaviour, IGridSystem
{
    const int HEIGHT = 7; 
    const int WIDTH = 7; 

    Grid grid;

    [SerializeField] GameObject gridPrefab;

    private GridCellHandler gridCellHandler; 

    public void Init(GridCellHandler gridCellHandler)
    {
        this.gridCellHandler = gridCellHandler;

        float gridSize = gridPrefab.GetComponent<BoxCollider>().size.x;

        grid = new Grid(HEIGHT, WIDTH, gridSize, Vector3.zero, gridPrefab, gridCellHandler);

        grid.CreateGridMap(this.transform);

        foreach (GridCell gridCell in grid.GetAllCells())
            gridCell.OnClicked += HandleGridCell;

    }

    #region Action 
    /// <summary>
    /// 액션이 선택되었을 때 
    /// </summary>
    public event Action<Vector2Int> OnActionOccured;
    
    /// <summary>
    /// 선택된 GridCell로 액션을 수행. 
    /// </summary>
    /// <param name="gridCell">선택된 GridCell</param>
    public void HandleGridCell(GridCell gridCell)
    {
        Vector2Int gridPosition = gridCell.GetGridPosition();

        if (!gridCell.isHighlighted)
        {
            GameObject.FindAnyObjectByType<ActionSystem>()?.CancelAction();
            return; 
        }
        
        OnActionOccured?.Invoke(gridPosition);
    }
    #endregion 

    #region Highlight
    public void HighlightGridCells(Predicate<Vector2Int> predicate, HighlightType type, HighlightLayer layer)
    {
        // Predicate는 delegate의 일종으로 bool type을 리턴값으로 가진다. 
        foreach(GridCell gridCell in grid.GetAllCells())
        {
            if (predicate(gridCell.GetGridPosition()))
                gridCell.Highlight(type, layer);
        }
    }
    public void UnhighlightGridCells(HighlightLayer layer)
    {
        for (int i = 0; i < HEIGHT; i++)
        {
            for (int j = 0; j < WIDTH; j++)
            {
                Vector2Int pos = new Vector2Int(j, i);
                GridCell gridCell = grid.GetGridCell(pos);
                gridCell.Unhighlight(layer); 
            }
        }
    }
    
    #endregion

    #region Grid Placement 
    public void PlaceObjectTo(GameObject obj, Vector2Int gridPosition)
    {
        grid.PlaceObjectTo(obj, gridPosition); 
    }
    public void RemoveObjectFrom(GameObject obj, Vector2Int gridPosition)
    {
        grid.RemoveObjectFrom(obj, gridPosition); 
    }
    public void MoveObjectFrom(Vector2Int from, Vector2Int to)
    {
        grid.MoveObject(from, to); 
    }
    public bool IsObjectOnGridPosition(Vector2Int gridPosition)
    {
        if (grid.GetObjectOnGridCell(gridPosition) == null)
            return false;
        else
            return true; 
    }
    #endregion

    #region Get Functions 
    public Vector3 GetWorldPosition(Vector2Int gridPosition) => grid.GetWorldPosition(gridPosition); 
    public GameObject GetGameObjectOnGrid(Vector2Int gridPosition)
    {
        return grid.GetObjectOnGridCell(gridPosition); 
    }
    public Vector2Int GetGridPositionOfGameObject(GameObject go)
    {
        for(int i=0; i<HEIGHT; i++)
        {
            for(int j=0; j<WIDTH; j++)
            {
                Vector2Int gridPosition = new Vector2Int(j, i); 
                if(go == grid.GetObjectOnGridCell(gridPosition))
                {
                    return gridPosition; 
                }
            }
        }

        return -Vector2Int.one; 
    }
    public GridCell GetGridCell(Vector2Int gridPosition) => grid.GetGridCell(gridPosition);
    public Vector2Int GetGridPosition(Vector3 worldPosition)
    {
        return grid.GetGridPosition(worldPosition);
    }
    #endregion

    public bool IsValidPosition(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < WIDTH && pos.y >= 0 && pos.y < HEIGHT;
    }
}