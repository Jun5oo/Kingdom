using System;
using UnityEngine;

public enum InteractionState
{
    Idle, 
    Hover, 
    Selected, 
    Moving
}

public abstract class BaseInteraction : MonoBehaviour, IHoverable, ISelectable
{
    private BaseObject baseObject; 
    protected InteractionState currentState;

    protected Vector3 originPos;
    protected Quaternion originRotation;
    protected Vector3 originScale;

    public virtual BaseObject BaseObject { get { return baseObject; } }

    public event Action OnSelectedComplete;

    public virtual void Init(BaseObject baseObject)
    {
        this.baseObject = baseObject; 
        currentState = InteractionState.Idle;
    }

    public virtual bool IsHoverable() => currentState == InteractionState.Hover;
    public virtual bool IsSelectable() => currentState == InteractionState.Idle || currentState == InteractionState.Hover; 

    public abstract void OnHover();
    public abstract void OffHover();
    public abstract void OnSelected();
    public abstract void OnDeselected();
    
    public virtual void OnMoveComplete()
    {
        if (currentState == InteractionState.Moving)
            currentState = InteractionState.Idle; 
    }
    public virtual void OnSelectionComplete()
    {
        OnSelectedComplete?.Invoke(); 
    }
    public virtual void OnUpdatePRS()
    {
        currentState = InteractionState.Moving; 
    }
    public virtual void UnSubscribe()
    {
        OnSelectedComplete = null;
    }
}
