using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 지정된 크기의 그리드를 생성하고 그리드 좌표 ↔ 월드 좌표 변환을 제공하는 클래스.
/// 셀 크기는 프리팹의 BoxCollider.size.x에서 자동으로 읽는다.
/// 그리드 좌표 범위: (0,0) ~ (width-1, height-1).
/// </summary>
public class Grid
{
    private int height;
    private int width;

    private Vector3 originPos; // 그리드 중심 오프셋
    private GameObject prefab;
    private float prefabSize; // BoxCollider.size.x에서 읽은 셀 한 칸 크기

    public Action<GridCell> OnGridCellClicked;

    Dictionary<Vector2Int, GridCell> positionToCell; // 그리드 좌표 → GridCell
    List<GridCell> cellList;

    /// <summary> 그리드 크기, 원점, 프리팹을 설정하고 셀 크기를 BoxCollider에서 읽는다. </summary>
    public void Init(int width, int height, Vector3 originPos, GameObject prefab)
    {
        this.width = width;
        this.height = height;
        this.originPos = originPos;
        this.prefab = prefab;

        this.prefabSize = prefab.GetComponent<BoxCollider>().size.x;
    }

    #region Create Grid
    /// <summary> width × height 크기의 GridCell을 생성하고 딕셔너리에 등록한다. </summary>
    public void CreateGridMap(Transform gridParent)
    {
        positionToCell = new Dictionary<Vector2Int, GridCell>();
        cellList = new List<GridCell>();

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

                gridCell.OnClicked += OnClicked;
                positionToCell.Add(gridPos, gridCell);
                cellList.Add(gridCell);
            }
        }
    }
    #endregion

    #region GetFunctions
    /// <summary>
    /// 그리드 좌표를 월드 좌표로 변환한다.
    /// 그리드를 originPos 기준으로 중앙 정렬하고 각 셀 중심에 배치한다.
    /// </summary>
    public Vector3 GetWorldPosition(Vector2Int gridPosition)
    {
        float totalWidth = width * prefabSize;
        float totalHeight = height * prefabSize;

        float offsetX = -(totalWidth / 2) + (gridPosition.x * prefabSize) + (prefabSize / 2);
        float offsetZ = -(totalHeight / 2) + (gridPosition.y * prefabSize) + (prefabSize / 2);

        return new Vector3(offsetX + originPos.x, 0f, offsetZ + originPos.z);
    }

    /// <summary>
    /// 월드 좌표를 그리드 좌표로 변환한다.
    /// 그리드 범위 밖이면 -Vector2Int.one을 반환한다.
    /// </summary>
    public Vector2Int GetGridPosition(Vector3 worldPosition)
    {
        Vector3 relativePosition = worldPosition - originPos;

        float totalWidth = width * prefabSize;
        float totalHeight = height * prefabSize;

        float normalizedX = relativePosition.x + (totalWidth / 2);
        float normalizedZ = relativePosition.z + (totalHeight / 2);

        int gridX = Mathf.FloorToInt(normalizedX / prefabSize);
        int gridZ = Mathf.FloorToInt(normalizedZ / prefabSize);

        bool isGridSelected = gridX >= 0 && gridZ >= 0 && gridX < width && gridZ < height;

        if (!isGridSelected)
            return -Vector2Int.one;

        return new Vector2Int(gridX, gridZ);
    }

    /// <summary> 그리드 좌표에 해당하는 GridCell을 반환한다. 없으면 null을 반환한다. </summary>
    public GridCell GetGridCell(Vector2Int gridPosition)
    {
        if (positionToCell.TryGetValue(gridPosition, out GridCell cell))
            return cell;
        else
            return null;
    }

    public List<GridCell> GetAllCells() => cellList;
    #endregion

    void OnClicked(GridCell gridCell)
    {
        OnGridCellClicked?.Invoke(gridCell);
    }
}
