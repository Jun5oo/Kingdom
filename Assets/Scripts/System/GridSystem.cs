using System;
using UnityEngine;

public class GridSystem : MonoBehaviour, IGridSystem
{
    const int HEIGHT = 8; 
    const int WIDTH = 8; 

    Grid grid;

    [SerializeField] GameObject gridPrefab;

    void Awake()
    {
        float gridSize = gridPrefab.GetComponent<BoxCollider>().size.x;

        grid = new Grid(HEIGHT, WIDTH, gridSize, Vector3.zero, gridPrefab);
       
        grid.CreateGridMap(this.transform);
        
        foreach(GridCell gridCell in grid.GetAllCells())
            gridCell.OnClicked += HandleGridCell; 
    }

    #region Action 
    public event Action<Vector2Int> OnActionOccured;
    public void HandleGridCell(GridCell gridCell)
    {
        Vector2Int gridPosition = gridCell.GetGridPosition();

        if (!gridCell.isHighlighted)
            return; 

        OnActionOccured?.Invoke(gridPosition);
    }
    #endregion 

    #region Highlight
    public void HighlightGridCells(Predicate<Vector2Int> predicate)
    {
        // Predicate�� delegate�� �������� bool type ���ϰ��� ������. 
        foreach(GridCell gridCell in grid.GetAllCells())
        {
            if (predicate(gridCell.GetGridPosition()))
                gridCell.Highlight(); 
        }
    }
    public void UnhighlightGridCells()
    {
        for (int i = 0; i < HEIGHT; i++)
        {
            for (int j = 0; j < WIDTH; j++)
            {
                Vector2Int pos = new Vector2Int(j, i);
                GridCell gridCell = grid.GetGridCell(pos);
                gridCell.Unhighlight(); 
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
    #endregion

    public Vector2Int GetGridPosition(Vector3 worldPosition)
    {
        return grid.GetGridPosition(worldPosition);
    }
    public bool IsValidPosition(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < WIDTH && pos.y >= 0 && pos.y < HEIGHT;
    }
}