using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 7x7 그리드를 생성하고 하이라이트 상태를 관리하는 매니저.
/// Action 레이어(액션 범위)와 Hover 레이어(마우스 오버)를 독립적으로 관리한다.
/// GridCell 클릭 이벤트를 수신하여 OnGridCellSelected로 ActionSystem에 전달한다.
/// </summary>
public class GridManager : MonoBehaviour
{
    public const int HEIGHT = 7;
    public const int WIDTH = 7;

    Vector3 originPos = Vector3.left * 8; // 그리드 좌측 하단 원점 오프셋

    Grid grid;

    [SerializeField] GameObject gridPrefab;

    Dictionary<HighlightLayer, List<GridCell>> highlightLayerToGridCells; // 레이어별 하이라이트된 셀 목록

    /// <summary> 그리드를 생성하고 각 셀의 클릭 이벤트를 등록한다. </summary>
    public void Init()
    {
        grid = new Grid();
        grid.Init(HEIGHT, WIDTH, originPos, gridPrefab);
        grid.CreateGridMap(transform);

        foreach (GridCell gridCell in grid.GetAllCells())
            gridCell.OnClicked += TrySelectGridCell;

        highlightLayerToGridCells = new Dictionary<HighlightLayer, List<GridCell>>();

        foreach (HighlightLayer layer in Enum.GetValues(typeof(HighlightLayer)))
            highlightLayerToGridCells[layer] = new List<GridCell>();
    }

    public event Action<Vector2Int> OnGridCellSelected; // 유효한 하이라이트 셀 클릭 시 발생

    /// <summary>
    /// 클릭된 셀이 Action 레이어에 하이라이트된 셀인 경우에만 OnGridCellSelected를 발생시킨다.
    /// 하이라이트되지 않은 셀 클릭은 무시한다.
    /// </summary>
    public void TrySelectGridCell(GridCell gridCell)
    {
        List<GridCell> highlightedCells = highlightLayerToGridCells[HighlightLayer.Action];

        if (!highlightedCells.Contains(gridCell))
            return;

        Vector2Int gridPosition = gridCell.GetGridPosition();
        OnGridCellSelected?.Invoke(gridPosition);
    }

    #region Highlight
    /// <summary>
    /// predicate 조건을 만족하는 모든 셀을 지정 타입/레이어로 하이라이트한다.
    /// </summary>
    public void HighlightGridCells(Predicate<Vector2Int> predicate, HighlightType type, HighlightLayer layer)
    {
        foreach (GridCell gridCell in grid.GetAllCells())
        {
            if (predicate(gridCell.GetGridPosition()))
            {
                gridCell.Highlight(type, layer);
                highlightLayerToGridCells[layer].Add(gridCell);
            }
        }
    }

    /// <summary> predicate 조건을 만족하는 그리드 좌표 목록을 반환한다. </summary>
    public List<Vector2Int> GetSummonableGridCells(Predicate<Vector2Int> predicate)
    {
        List<Vector2Int> summonablePosList = new List<Vector2Int>();

        foreach (GridCell gridCell in grid.GetAllCells())
        {
            Vector2Int gridPos = gridCell.GetGridPosition();
            if (predicate(gridPos))
                summonablePosList.Add(gridPos);
        }

        return summonablePosList;
    }

    /// <summary> 지정 레이어의 모든 하이라이트를 제거한다. </summary>
    public void UnhighlightGridCells(HighlightLayer layer)
    {
        List<GridCell> highlightedCells = highlightLayerToGridCells[layer];

        foreach (GridCell highlightCell in highlightedCells)
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