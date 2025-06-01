using NUnit.Framework;
using System;
using UnityEngine;
public class ActionSystem : MonoBehaviour, IActionSystem
{
    [SerializeField] IAction currentAction;

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
        this.currentAction?.Enter(); 
    }
    public void CancelAction()
    {
        currentAction?.Exit();
        currentAction = null;
    }
    public bool IsActionInProgress() => currentAction == null ? false : true;
    #endregion

    public IAction Create(IGridSystem gridSystem, Card card, ActionType actionType)
    {
        IAction action = null; 

        switch (actionType)
        {
            case ActionType.Summon:
                action = new SummonAction(gridSystem, this, card);
                break;
            case ActionType.Move: 
                action = new MoveAction(gridSystem, this, card);
                break;
            case ActionType.Attack:  
                action = new AttackAction(gridSystem, this, card);
                break; 
        }

        if(action == null)
        {
            Debug.LogError("No such action exists");
            return null; 
        }

        else
            Debug.Log(action); 

        return action; 
    }
}
