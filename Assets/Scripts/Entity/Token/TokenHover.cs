using System;
using UnityEngine;

public class TokenHover : EntityHover
{
    [SerializeField] Token token;
    [SerializeField] TokenMovement tokenMovement;

    public override Entity Entity { get { return token; } }
    
    public Action<Token> OnTokenSelected;
    public Action OnTokenDeselected; 

    float selectOffsetY = 0.5f; 

    public override void Init()
    {
        base.Init(); 

        tokenMovement.OnMoved -= OnUpdatePRS;
        tokenMovement.OnMoved += OnUpdatePRS;

        tokenMovement.OnMovedComplete -= OnMoveComplete;
        tokenMovement.OnMovedComplete += OnMoveComplete; 

        originPos = tokenMovement.PRS.position;
        originRotation = tokenMovement.PRS.rotation; 
        originScale = tokenMovement.PRS.scale;
    }

    public override void OnHover()
    {
        if (!IsHoverable())
            return;

        hoverState = HoverState.Hover;
    }
    public override void OffHover()
    {
        if (hoverState != HoverState.Hover)
            return;

        hoverState = HoverState.Idle; 
    }
    public override void OnSelected()
    {
        if (!IsSelectable())
            return;

        Vector3 position = originPos + (Vector3.up * selectOffsetY); 
        Quaternion rotation = originRotation;
        Vector3 scale = originScale;

        tokenMovement.MoveTransform(new PRS(position, rotation, scale), 0.2f, true, () =>
        {
            hoverState = HoverState.Selected;
            OnTokenSelected?.Invoke(token);
            OnSelectionComplete(); 
        }); 
    }
    public override void OnDeselected()
    {
        if (hoverState != HoverState.Selected)
            return;

        Vector3 position = originPos;
        Quaternion rotation = originRotation;
        Vector3 scale = originScale;

        tokenMovement.MoveTransform(new PRS(position, rotation, scale), 0.2f, true, () =>
        {
            hoverState = HoverState.Idle;
            OnTokenDeselected?.Invoke(); 
        });
    }
    public override void OnUpdatePRS()
    {
        base.OnUpdatePRS(); 

        PRS prs = tokenMovement.PRS; 

        originPos = prs.position;
        originRotation = prs.rotation;
        originScale = prs.scale;
    }
    void OnDestroy()
    {
        UnSubscribe();

        tokenMovement.OnMoved -= OnUpdatePRS;
        tokenMovement.OnMovedComplete -= OnMoveComplete;
        OnTokenSelected = null;
        OnTokenDeselected = null;
    }
}
