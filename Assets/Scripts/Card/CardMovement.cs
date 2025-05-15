using DG.Tweening;
using System;
using UnityEngine;

public class CardMovement : MonoBehaviour
{
    public PRS prs;
    public PRS PRS
    {
        get { return prs; }
        set
        {
            prs = value;
            OnCardMoved?.Invoke(prs); 
        }
    }
    public bool isMoving = false;
    public Action<PRS> OnCardMoved; 

    public void MoveTransform(PRS targetPRS, float duration, bool isHover = false)
    {
        if (!isHover)
            PRS = targetPRS;
        else
            prs = targetPRS; 
        
        isMoving = true;

        Sequence sequence = DOTween.Sequence();

        sequence.Append(transform.DOMove(prs.position, duration));
        sequence.Join(transform.DORotateQuaternion(prs.rotation, duration));
        sequence.Join(transform.DOScale(prs.scale, duration));

        sequence.OnComplete(() => isMoving = false);
    }
}
