using System;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public const int HEIGHT = 7; 
    public const int WIDTH = 7;
    const int OFFSET_X = 8;
    
    Grid grid;

    Vector3 originPos;

    [SerializeField] GameObject gridPrefab;

    Dictionary<HighlightLayer, List<GridCell>> highlightLayerToGridCells; 

    public void Init()
    {
        grid = new Grid();

        originPos = Vector3.left * OFFSET_X; 

        grid.Init(HEIGHT, WIDTH, originPos, gridPrefab);

        grid.CreateGridMap(transform);

        foreach (GridCell gridCell in grid.GetAllCells())
            gridCell.OnClicked += TrySelectGridCell;

        highlightLayerToGridCells = new Dictionary<HighlightLayer, List<GridCell>>();
        
        foreach (HighlightLayer layer in Enum.GetValues(typeof(HighlightLayer)))
            highlightLayerToGridCells[layer] = new List<GridCell>();
    }
    
    public event Action<Vector2Int> OnGridCellSelected;
    public void TrySelectGridCell(GridCell gridCell)
    {
        List<GridCell> highlightedCells = highlightLayerToGridCells[HighlightLayer.Action]; 

        if (!highlightedCells.Contains(gridCell))
            return;

        Vector2Int gridPosition = gridCell.GetGridPosition(); 
        
        OnGridCellSelected?.Invoke(gridPosition);
    }
    
    #region Highlight
    public void HighlightGridCells(Predicate<Vector2Int> predicate, HighlightType type, HighlightLayer layer)
    {
        // Predicate는 delegate의 일종으로 bool type을 리턴값으로 가진다. 
        foreach(GridCell gridCell in grid.GetAllCells())
        {
            if (predicate(gridCell.GetGridPosition()))
            {
                gridCell.Highlight(type, layer);
                highlightLayerToGridCells[layer].Add(gridCell); 
            }
        }
    }

    public List<Vector2Int> GetSummonableGridCells(Predicate<Vector2Int> predicate)
    {
        List<Vector2Int> summonablePosList = new List<Vector2Int>();

        foreach (GridCell gridCell in grid.GetAllCells())
        {
            Vector2Int gridPos = gridCell.GetGridPosition();
            if (predicate(gridPos))
            {
                summonablePosList.Add(gridPos);
            }
        }

        return summonablePosList;
    }

    public void UnhighlightGridCells(HighlightLayer layer)
    {
        List<GridCell> highlightedCells = highlightLayerToGridCells[layer];

        foreach(GridCell highlightCell in highlightedCells)
            highlightCell.Unhighlight(layer);

        highlightedCells.Clear();
    }
    #endregion

    #region Get Functions 
    public Vector3 GetWorldPosition(Vector2Int gridPosition) => grid.GetWorldPosition(gridPosition);
    public Vector2Int GetGridPosition(Vector3 worldPosition) => grid.GetGridPosition(worldPosition);
    public GridCell GetGridCell(Vector2Int gridPosition) => grid.GetGridCell(gridPosition);
    public int GetRandomGridXPos() => UnityEngine.Random.Range(0, WIDTH - 1);
    public int GetRandomGridYPos() => UnityEngine.Random.Range(0, HEIGHT - 1);
    #endregion
}