using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

/// <summary>
/// 플레이어가 그리드 셀을 선택할 때까지 비동기로 대기하는 유틸리티.
/// 조건에 맞는 셀을 하이라이트하고 GridManager.OnGridCellSelected 이벤트를 기다린다.
/// CancellationToken으로 외부에서 취소하면 TaskCompletionSource가 Canceled 상태가 된다.
/// </summary>
public class GridSelection
{
    GridManager gridManager;

    Predicate<Vector2Int> selectableGrid; // 선택 가능한 셀 조건
    HighlightLayer highlightLayer;

    UniTaskCompletionSource<Vector2Int> taskCompletionSource;
    CancellationTokenRegistration ctr;

    bool isActive;

    public void Init()
    {
        gridManager = ServiceLocator.Get<GridManager>();
        isActive = false;
    }

    /// <summary>
    /// 조건에 맞는 셀을 하이라이트하고 선택 이벤트를 대기하는 UniTask를 반환한다.
    /// 이미 대기 중이면 예외를 발생시킨다.
    /// </summary>
    public UniTask<Vector2Int> WaitGridSelectionAsync(Predicate<Vector2Int> predicate, HighlightType type, HighlightLayer layer, CancellationToken ct = default)
    {
        if (isActive)
            throw new Exception("GridSelection: 기다리는 중");

        isActive = true;

        taskCompletionSource = new UniTaskCompletionSource<Vector2Int>();

        this.selectableGrid = predicate;
        this.highlightLayer = layer;

        gridManager?.HighlightGridCells(predicate, type, layer);
        gridManager.OnGridCellSelected += OnSelect;

        if (ct.CanBeCanceled)
            ctr = ct.Register(OnCancel);

        return taskCompletionSource.Task;
    }

    /// <summary> 선택된 셀이 조건을 만족하면 결과를 설정하고 대기를 완료한다. </summary>
    void OnSelect(Vector2Int pos)
    {
        if (!isActive || selectableGrid == null)
            return;

        if (!selectableGrid(pos))
            return;

        Clean();
        taskCompletionSource.TrySetResult(pos);
    }

    /// <summary> CancellationToken이 취소되면 대기를 Canceled 상태로 종료한다. </summary>
    void OnCancel()
    {
        if (!isActive)
            return;

        Clean();
        taskCompletionSource.TrySetCanceled();
    }

    /// <summary> 이벤트 구독 해제, 하이라이트 제거, 상태 초기화를 수행한다. </summary>
    void Clean()
    {
        if (gridManager != null)
            gridManager.OnGridCellSelected -= OnSelect;

        gridManager?.UnhighlightGridCells(highlightLayer);
        ctr.Dispose();
        selectableGrid = null;
        isActive = false;
    }
}
