using System;
using System.Collections.Generic;
using UnityEngine;

public class Grid
{
    private int height;
    private int width;
    private float cellSize;

    private Vector3 originPos;
    private GameObject prefab;

    #region Dictionaries 
    Dictionary<Vector2Int,  GridCell> positionToCell;
    Dictionary<Vector2Int, GameObject> objectOnGrid;
    #endregion 

    List<GridCell> cellList; 

    #region Constructor 
    public Grid(int width, int height, float cellSize, Vector3 originPos, GameObject prefab)
    {
        this.width = width; 
        this.height = height;
        this.cellSize = cellSize;
        this.originPos = originPos; 
        this.prefab = prefab; 
    }
    #endregion 

    #region Create Grid 
    public void CreateGridMap(Transform gridParent)
    {
        positionToCell = new Dictionary<Vector2Int, GridCell>(); 
        objectOnGrid = new Dictionary<Vector2Int, GameObject>();

        cellList = new List<GridCell>(); 

        // GridPosition: (0, 0) ~ (width - 1,  height - 1); 

        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                GameObject gridObject = GameObject.Instantiate(prefab, GetWorldPosition(new Vector2Int(j, i)), Quaternion.Euler(90, 0, 0));
                gridObject.name = $"{j},{i}";
                gridObject.transform.parent = gridParent.transform;

                Vector2Int gridPos = new Vector2Int(j, i); 

                GridCell gridCell = gridObject.GetComponent<GridCell>();
                gridCell.Init(gridPos);

                gridCell.OnClicked += HandleGridCellClicked; 

                // isMyCell = �� ī�带 ���� �� �ִ� Cell
                if (i < 3)
                    gridCell.isMyCell = true; 
                else 
                    gridCell.isMyCell = false;

                positionToCell.Add(gridPos, gridCell);
                objectOnGrid.Add(gridPos, null); 

                cellList.Add(gridCell);
            }
        }
    }
    #endregion

    #region Get Functions 
    public Vector3 GetWorldPosition(Vector2Int gridPosition)
    {
        float totalWidth = width * cellSize;
        float totalHeight = height * cellSize;

        float offsetX = -(totalWidth / 2) + (gridPosition.x * cellSize) + (cellSize / 2);
        float offsetZ = -(totalHeight / 2) + (gridPosition.y * cellSize)  + (cellSize / 2); 

        return new Vector3(offsetX + originPos.x, 0f, offsetZ + originPos.z);
    }
    public Vector2Int GetGridPosition(Vector3 worldPosition)
    {
        Vector3 relativePosition = worldPosition - originPos; 

        float totalWidth = width * cellSize; 
        float totalHeight = height * cellSize;

        float normalizedX = relativePosition.x + (totalWidth / 2);
        float normalizedZ = relativePosition.z + (totalHeight / 2);

        int gridX = Mathf.FloorToInt(normalizedX/cellSize);
        int gridZ = Mathf.FloorToInt(normalizedZ/cellSize);

        bool isGridSelected = gridX >= 0 && gridZ >= 0 && gridX < width && gridZ < height;

        if (!isGridSelected)
            return -Vector2Int.one; 

        return new Vector2Int(gridX, gridZ);
    }
    public GridCell GetGridCell(Vector2Int gridPosition)
    {
        if (positionToCell.TryGetValue(gridPosition, out GridCell cell))
            return cell; 
        else
            return null; 
    }
    public GameObject GetObjectOnGridCell(Vector2Int gridPosition)
    {
        return objectOnGrid[gridPosition]; 
    }
    public List<GridCell> GetAllCells() => cellList; 
    #endregion

    #region Grid Placement 
    public void PlaceObjectTo(GameObject obj, Vector2Int gridPosition)
    {
        if (objectOnGrid.ContainsKey(gridPosition))
            objectOnGrid[gridPosition] = obj; 
    }
    public void RemoveObjectFrom(GameObject obj, Vector2Int gridPosition)
    {
        if(objectOnGrid.TryGetValue(gridPosition, out GameObject gameObject))
            objectOnGrid[gridPosition] = null; 
    }
    public void MoveObject(Vector2Int from, Vector2Int to)
    {
        GameObject gameObject = GetObjectOnGridCell(from);
        RemoveObjectFrom(gameObject, from);
        PlaceObjectTo(gameObject, to); 
    }
    #endregion

    #region Actions 
    public Action<GridCell> OnGridCellClicked;
    public void HandleGridCellClicked(GridCell gridCell)
    {
        OnGridCellClicked?.Invoke(gridCell);
    }
    #endregion 
}
