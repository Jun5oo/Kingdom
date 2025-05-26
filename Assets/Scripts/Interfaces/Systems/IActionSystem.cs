using System;

public interface IActionSystem
{
    public void EnterAction(IAction action);
    public void CancelAction();
    public bool IsActionInProgress(); 

    public event Action OnCancelOccured; 
}
