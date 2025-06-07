using System;

public interface IActionSystem
{
    public void EnterAction(IAction action);
    public void CancelAction();
    public bool IsActionInProgress();
    public IAction Create(IGridSystem gridSystem, Card card, ActionType actionType); 
}
