using UnityEngine;

public class TokenInteraction : BaseInteraction
{
    [SerializeField] TokenMovement movement;

    public override void Init(BaseObject baseObject)
    {
        base.Init(baseObject); 

        movement.OnMoved -= OnUpdatePRS;
        movement.OnMoved += OnUpdatePRS;

        movement.OnMovedComplete -= OnMoveComplete;
        movement.OnMovedComplete += OnMoveComplete; 

        originPos = movement.PRS.position;
        originRotation = movement.PRS.rotation; 
        originScale = movement.PRS.scale;
    }

    public override void OnHover()
    {
        if (!IsHoverable())
            return;

        currentState = InteractionState.Hover;
    }
    public override void OffHover()
    {
        if (currentState != InteractionState.Hover)
            return;

        currentState = InteractionState.Idle; 
    }
    public override void OnSelected()
    {
        if (!IsSelectable())
            return;

        currentState = InteractionState.Selected;
        OnSelectionComplete();

    }
    public override void OnDeselected()
    {
        if (currentState != InteractionState.Selected)
            return;

        currentState = InteractionState.Idle;

    }
    public override void OnUpdatePRS()
    {
        base.OnUpdatePRS(); 

        PRS prs = movement.PRS; 

        originPos = prs.position;
        originRotation = prs.rotation;
        originScale = prs.scale;
    }
    void OnDestroy()
    {
        Clear(); 
    }

    void Clear()
    {
        UnSubscribe();
        movement.OnMoved -= OnUpdatePRS;
        movement.OnMovedComplete -= OnMoveComplete;
    }
}
