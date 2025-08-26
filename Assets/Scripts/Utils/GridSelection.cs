using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

public class GridSelection 
{
    GridManager gridManager;
    
    Predicate<Vector2Int> selectableGrid;
    HighlightLayer highlightLayer; 

    UniTaskCompletionSource<Vector2Int> taskCompletionSource;
    CancellationTokenRegistration ctr;

    bool isActive; 

    public void Init()
    {
        gridManager = ServiceLocator.Get<GridManager>();
        isActive = false; 
    }

    public UniTask<Vector2Int> WaitGridSelectionAsync(Predicate<Vector2Int> predicate, HighlightType type, HighlightLayer layer, CancellationToken ct = default )
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

    void OnSelect(Vector2Int pos)
    {
        if (!isActive || selectableGrid == null)
            return; 

        if (!selectableGrid(pos))
            return;

        Clean();
        taskCompletionSource.TrySetResult(pos); 
    }

    void OnCancel()
    {
        if (!isActive)
            return;

        Clean(); 
        taskCompletionSource.TrySetCanceled();
    }

    void Clean()
    {
        if(gridManager != null)
            gridManager.OnGridCellSelected -= OnSelect;
        
        gridManager?.UnhighlightGridCells(highlightLayer);
        ctr.Dispose(); 
        selectableGrid = null;
        isActive = false; 
    }
}
