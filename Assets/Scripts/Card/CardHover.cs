using UnityEngine;

public class CardHover : MonoBehaviour, IHoverable, ISelectable
{
    [SerializeField] Card card; 
    [SerializeField] CardMovement cardMovement;
    [SerializeField] Summon_Icon summonIcon; 

    Vector3 originPos;
    Quaternion originRotation;
    Vector3 originScale;

    // OnHover PRS 
    Vector3 hoverOffset = Vector3.up * 0.01f; 
    Vector3 hoverScale = Vector3.one * 1.1f; 

    // OnSelected PRS 
    Vector3 selectedOffset = Vector3.up * 0.1f + Vector3.forward * 0.5f;  
    Vector3 selectedScale = Vector3.one * 1.3f; 
    Quaternion selectedRotation = Quaternion.Euler(0f, 0f, -180f);

    bool isSelected = false; 

    public void OnHover()
    {
        // 
        GetOriginPRS(); 

        if (cardMovement.isMoving)
            return;

        Vector3 targetPosition = originPos + hoverOffset;
        Vector3 targetScale = hoverScale; 

        cardMovement.MoveTransform(new PRS(targetPosition, originRotation, targetScale), 0f);
    }

    public void OffHover()
    {
        cardMovement.MoveTransform(new PRS(originPos, originRotation, originScale), 0f); 
    }

    public void OnSelected()
    {
        GetOriginPRS(); 

        Vector3 targetPosition = originPos + selectedOffset;
        Quaternion targetRotation = selectedRotation;
        Vector3 targetScale = selectedScale;

        cardMovement.MoveTransform(new PRS(targetPosition, targetRotation, targetScale), 0f);
        summonIcon.gameObject.SetActive(true);
        isSelected = true;
    }
    public void OnDeselected()
    {
        OffHover();
        summonIcon.gameObject.SetActive(false);
        isSelected = false; 
    }
    public bool IsSelectable()
    {
        return !isSelected;  
    }

    void GetOriginPRS()
    {
        originPos = cardMovement.prs.position;
        originRotation = cardMovement.prs.rotation;
        originScale = cardMovement.prs.scale;
    }
}
