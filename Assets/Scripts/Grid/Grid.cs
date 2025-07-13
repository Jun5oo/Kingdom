using System;
using System.Collections.Generic;
using UnityEngine;

public class Grid
{
    private int height;
    private int width;

    private Vector3 originPos;
    private GameObject prefab;
    private float prefabSize;

    public Action<GridCell> OnGridCellClicked;

    Dictionary<Vector2Int,  GridCell> positionToCell;
    List<GridCell> cellList;

    public void Init(int width, int height, Vector3 originPos, GameObject prefab)
    {
        this.width = width; 
        this.height = height;
        this.originPos = originPos; 
        this.prefab = prefab;

        this.prefabSize = prefab.GetComponent<BoxCollider>().size.x; 
    }

    #region Create Grid 
    public void CreateGridMap(Transform gridParent, List<Sprite> sprites)
    {
        positionToCell = new Dictionary<Vector2Int, GridCell>(); 

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

                int idx = i * 7 + j; 

                gridCell.Init(gridPos, sprites[idx]);

                gridCell.OnClicked += OnClicked; 
                positionToCell.Add(gridPos, gridCell);
                cellList.Add(gridCell);
            }
        }
    }
    #endregion

    #region GetFunctions 
    public Vector3 GetWorldPosition(Vector2Int gridPosition)
    {
        float totalWidth = width * prefabSize;
        float totalHeight = height * prefabSize;

        float offsetX = -(totalWidth / 2) + (gridPosition.x * prefabSize) + (prefabSize / 2);
        float offsetZ = -(totalHeight / 2) + (gridPosition.y * prefabSize)  + (prefabSize / 2); 

        return new Vector3(offsetX + originPos.x, 0f, offsetZ + originPos.z);
    }
    public Vector2Int GetGridPosition(Vector3 worldPosition)
    {
        Vector3 relativePosition = worldPosition - originPos; 

        float totalWidth = width * prefabSize; 
        float totalHeight = height * prefabSize;

        float normalizedX = relativePosition.x + (totalWidth / 2);
        float normalizedZ = relativePosition.z + (totalHeight / 2);

        int gridX = Mathf.FloorToInt(normalizedX/prefabSize);
        int gridZ = Mathf.FloorToInt(normalizedZ/prefabSize);

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
    public List<GridCell> GetAllCells() => cellList; 
    #endregion

    public void OnClicked(GridCell gridCell)
    {
        OnGridCellClicked?.Invoke(gridCell);
    }
}
