using System;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    const int HEIGHT = 7; 
    const int WIDTH = 7; 

    Grid grid;

    [SerializeField] GameObject gridPrefab;
    [SerializeField] List<Sprite> gridCellSprite;

    Dictionary<HighlightLayer, List<GridCell>> highlightLayerToGridCells; 

    public void Init(TokenManager tokenManager)
    {
        grid = new Grid();
        grid.Init(HEIGHT, WIDTH, Vector3.zero, gridPrefab);
        grid.CreateGridMap(transform, gridCellSprite);

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
    #endregion
}