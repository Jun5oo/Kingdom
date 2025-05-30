using System;
using UnityEngine;
public class ActionSystem : MonoBehaviour, IActionSystem
{
    [SerializeField] IAction currentAction;

    public event Action OnCancelOccured;

    [SerializeField] string currentActionName; 
    
    void Update()
    {
        if (currentAction != null)
            currentActionName = currentAction.ToString();
        else
            currentActionName = "Null";

        if (Input.GetKeyDown(KeyCode.Mouse1))
            CancelAction(); 
    }

    #region Action
    public void EnterAction(IAction action)
    {
        if (!action.IsValid())
        {
            Debug.LogError("Action invalid!");
            return;
        }

        CancelAction(); 

        this.currentAction = action;
        this.currentAction.Enter(); 
    }
    public void CancelAction()
    {
        currentAction?.Exit();
        currentAction = null;
    }
    public bool IsActionInProgress() => currentAction == null ? false : true; 
    #endregion

}
