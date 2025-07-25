using UnityEngine;

public class CardInteraction : BaseInteraction
{
    [SerializeField] CardMovement movement;

    Vector3 hoverOffset;
    Vector3 hoverScale;

    Vector3 selectedOffset;
    Vector3 selectedScale;

    public override void Init(BaseObject baseObject)
    {
        base.Init(baseObject); 

        movement.OnMoved -= OnUpdatePRS; 
        movement.OnMoved += OnUpdatePRS;
        
        movement.OnMovedComplete -= OnMoveComplete;
        movement.OnMovedComplete += OnMoveComplete;

        originScale = Vector3.one; 

        hoverOffset = Vector3.up * 0.01f;
        hoverScale = originScale * 1.1f;

        selectedOffset = Vector3.up * 0.5f + Vector3.forward * 0.5f;
        selectedScale = originScale * 1.3f;
    }

    #region Hover 
    public override void OnHover()
    {
        if (!IsHoverable())
            return; 

        Vector3 targetPosition = originPos + hoverOffset;
        Vector3 targetScale = hoverScale;

        movement.MoveTransform(new PRS(targetPosition, originRotation, targetScale), 0f, true, () => { currentState = InteractionState.Hover; });
    }
    public override void OffHover()
    {
        if (currentState != InteractionState.Hover)
            return; 

        movement.MoveTransform(new PRS(originPos, originRotation, originScale), 0f, true, () => { currentState = InteractionState.Idle; }); 
    }
    #endregion

    #region Selection
    public override void OnSelected()
    {
        if (!IsSelectable())
            return; 

        Vector3 targetPosition = originPos + selectedOffset;
        Quaternion targetRotation = originRotation;
        Vector3 targetScale = selectedScale;

        movement.MoveTransform(new PRS(targetPosition, targetRotation, targetScale), 0.2f, true, () => 
        {
            currentState = InteractionState.Selected; 
            OnSelectionComplete(); 
        });

        Debug.Log(currentState); 
    }
    public override void OnDeselected()
    {
        if (currentState != InteractionState.Selected)
            return; 

        movement.MoveTransform(new PRS(originPos, originRotation, originScale), 0.2f, true, ()=> { currentState = InteractionState.Idle; });
    }
    #endregion

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
        UnSubscribe();
        movement.OnMoved -= OnUpdatePRS;
        movement.OnMovedComplete -= OnMoveComplete;
    }
}
