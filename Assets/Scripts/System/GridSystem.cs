using UnityEngine;

public class GridSystem : MonoBehaviour
{
    const int HEIGHT = 8; 
    const int WIDTH = 8; 

    Grid grid;

    [SerializeField] GameObject gridPrefab;

    void Awake()
    {
        if (gridPrefab == null)
        {
            Debug.LogError("Grid Prefab is not found");
            return;
        }

        float gridSize = gridPrefab.GetComponent<BoxCollider>().size.x;

        grid = new Grid(HEIGHT, WIDTH, gridSize, Vector3.zero, gridPrefab);
        grid.CreateGridMap(this.transform);
    }

    public void HighLightGridCell()
    {
        for(int i=0; i< HEIGHT; i++)
        {
            for(int j=0; j< WIDTH; j++)
            {
                Vector2Int gridPos = new Vector2Int(j, i);
                GameObject cellObject = grid.GetObjectOnGridCell(gridPos); 
                GridCell gridCell = cellObject.GetComponent<GridCell>();

                if (cellObject == null)
                    gridCell.HighLightValid();
                else
                    gridCell.HighLightInvalid(); 
            }
        }
    }

    public Vector3 GetSingleTestGrid()
    {
        int x = Random.Range(0, 8);
        int y = Random.Range(0, 5); 

        Vector2Int vec2Int = new Vector2Int(x, y);
        Vector3 pos = grid.GetWorldPosition(vec2Int);
        return pos; 
    }
}