using UnityEngine;

public interface IAction
{
    /// <summary>
    /// 액션 인터페이스 
    /// </summary>
    
    public ActionType ActionType { get; }
    public void Enter();
    public void Exit();
    public bool IsValid(); 
}
