using System.Collections.Generic;
using UnityEngine;

public class Grid
{
    private int height;
    private int width;
    private float cellSize;

    private Vector3 originPos;
    private GameObject prefab; 

    // key: gridPosition, value: gridObject 
    Dictionary<Vector2Int, GameObject> positionToObject;
    Dictionary<GameObject, Vector2Int> objectToPosition;
    // Test 
    Dictionary<Vector2Int, GameObject> objectOnGrid;

    public Grid(int width, int height, float cellSize, Vector3 originPos, GameObject prefab)
    {
        this.width = width; 
        this.height = height;
        this.cellSize = cellSize;
        this.originPos = originPos; 
        this.prefab = prefab; 
    }
    public void CreateGridMap(Transform gridParent)
    {
        positionToObject = new Dictionary<Vector2Int, GameObject>(); 
        objectToPosition = new Dictionary<GameObject, Vector2Int>();
        objectOnGrid = new Dictionary<Vector2Int, GameObject>(); 

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

                positionToObject.Add(gridPos, gridObject);
                objectToPosition.Add(gridObject, gridPos);
                objectOnGrid.Add(gridPos, null); 
            }
        }
    }
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
    public Vector2Int GetGridPosition(GameObject obj)
    {
        if (objectToPosition.TryGetValue(obj, out Vector2Int position))
            return position;
        else
            return -Vector2Int.one; 
    }
    public GameObject GetGridCell(Vector2Int gridPosition)
    {
        if (positionToObject.TryGetValue(gridPosition, out GameObject obj))
            return obj; 
        else
            return null; 
    }
    public GameObject GetObjectOnGridCell(Vector2Int gridPosition)
    {
        return objectOnGrid[gridPosition]; 
    }

}
