using UnityEngine;

public class CardHover : MonoBehaviour, IHoverable, ISelectable
{
    [SerializeField] Card card; 
    [SerializeField] CardMovement cardMovement;

    [SerializeField] Vector3 originPos;
    [SerializeField] Quaternion originRotation;
    [SerializeField] Vector3 originScale;

    // OnHover PRS 
    Vector3 hoverOffset = Vector3.up * 0.01f; 
    Vector3 hoverScale = Vector3.one * 1.1f; 

    // OnSelected PRS 
    Vector3 selectedOffset = Vector3.up * 0.5f + Vector3.forward * 0.5f;  
    Vector3 selectedScale = Vector3.one * 1.3f; 
    Quaternion selectedRotation = Quaternion.Euler(0f, 0f, -180f);

    bool isSelected = false;

    void Awake()
    {
        cardMovement.OnCardMoved += SetPRS;
    }

    #region Hover 
    public void OnHover()
    {
        if (cardMovement.isMoving)
            return;

        Vector3 targetPosition = originPos + hoverOffset;
        Vector3 targetScale = hoverScale; 

        cardMovement.MoveTransform(new PRS(targetPosition, originRotation, targetScale), 0f, true);
    }
    public void OffHover()
    {
        cardMovement.MoveTransform(new PRS(originPos, originRotation, originScale), 0f, true); 
    }
    #endregion

    #region Selection
    public void OnSelected()
    {
        Vector3 targetPosition = originPos + selectedOffset;
        Quaternion targetRotation = selectedRotation;
        Vector3 targetScale = selectedScale;

        cardMovement.MoveTransform(new PRS(targetPosition, targetRotation, targetScale), 0f, true);
        isSelected = true;
    }
    public void OnDeselected()
    {
        cardMovement.MoveTransform(new PRS(originPos, originRotation, originScale), 0f, true); 
        isSelected = false; 
    }
    public bool IsSelectable()
    {
        return !isSelected;  
    }
    #endregion 
    public void SetPRS(PRS prs)
    {
        if(prs != null)
        {
            originPos = prs.position; 
            originRotation = prs.rotation;
            originScale = prs.scale; 
        }
    }
}
