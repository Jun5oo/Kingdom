using System;
using UnityEngine;

public interface IGridSystem
{
    public void HighlightGridCells(Predicate<Vector2Int> predicate);
    public void UnhighlightGridCells();
    public event Action<Vector2Int> OnActionOccured; 
    public bool IsObjectOnGridPosition(Vector2Int position);
    public void PlaceObjectTo(GameObject go, Vector2Int gridPosition);
    public void MoveObjectFrom(Vector2Int from, Vector2Int to);
    public Vector3 GetWorldPosition(Vector2Int gridPosition); 
    public GridCell GetGridCell(Vector2Int gridPosition);
    public Vector2Int GetGridPositionOfGameObject(GameObject go);
    public GameObject GetGameObjectOnGrid(Vector2Int gridPosition);
}
