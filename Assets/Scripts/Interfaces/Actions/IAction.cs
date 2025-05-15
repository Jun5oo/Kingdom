using UnityEngine;

public interface IAction
{
    public void Enter();
    public void Exit();
    public bool IsValid(); 
}
