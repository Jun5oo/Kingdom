using System;
using UnityEngine;

public enum HoverState
{
    Idle, 
    Hover, 
    Selected, 
    Moving
}

public abstract class EntityHover : MonoBehaviour, IHoverable, ISelectable
{
    public abstract Entity Entity { get; }

    [SerializeField] 
    protected HoverState hoverState;
    
    protected Vector3 originPos;
    protected Quaternion originRotation;
    protected Vector3 originScale; 

    public event Action OnSelectedComplete;

    public virtual void Init()
    {
        hoverState = HoverState.Idle;
    }
    public virtual bool IsHoverable()
    {
        return hoverState == HoverState.Idle;
    }
    public virtual bool IsSelectable()
    {
        return hoverState == HoverState.Idle || hoverState == HoverState.Hover; 
    }
    public abstract void OnHover();
    public abstract void OffHover();
    public abstract void OnSelected();
    public abstract void OnDeselected();
    public virtual void OnMoveComplete()
    {
        if (hoverState == HoverState.Moving)
            hoverState = HoverState.Idle; 
    }
    public virtual void OnSelectionComplete()
    {
        OnSelectedComplete?.Invoke(); 
    }
    public virtual void OnUpdatePRS()
    {
        hoverState = HoverState.Moving; 
    }
    public virtual void UnSubscribe()
    {
        OnSelectedComplete = null;
    }
}
