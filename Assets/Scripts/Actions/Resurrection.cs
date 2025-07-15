using System;
using UnityEngine;

public class Resurrection : IAction
{
    public ActionType ActionType => throw new NotImplementedException();

    public HighlightLayer HighlightLayer => throw new NotImplementedException();

    public HighlightType HighlightType => throw new NotImplementedException();

    public ActionPerformer Performer => throw new NotImplementedException();

    public event Action OnActionCanceled;
    public event Action OnActionComplete;

    public void Enter()
    {
        throw new NotImplementedException();
    }

    public void Execute(Vector2Int targetPosition)
    {
        throw new NotImplementedException();
    }

    public void Exit()
    {
        throw new NotImplementedException();
    }

    public bool IsValid()
    {
        throw new NotImplementedException();
    }
}
