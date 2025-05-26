using UnityEngine;

public interface IAction
{
    public ActionType ActionType { get; }
    public void Enter();
    public void Exit();
    public bool IsValid(); 
}
