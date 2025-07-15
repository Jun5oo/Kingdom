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
            OnCardMoved?.Invoke(); 
        }
    }

    public Action OnCardMoved;
    public Action OnCardMoveComplete; 

    Sequence sequence; 

    public void MoveTransform(PRS targetPRS, float duration, bool isHover = false, Action callback = null)
    {
        if (!isHover)
            PRS = targetPRS;

        OnCardMoved.Invoke();

        sequence = DOTween.Sequence(); 
        sequence.Append(transform.DOMove(targetPRS.position, duration));    
        sequence.Join(transform.DORotateQuaternion(targetPRS.rotation, duration));
        sequence.Join(transform.DOScale(targetPRS.scale, duration));

        sequence.OnComplete(() =>
        {
            callback?.Invoke();
            OnCardMoveComplete?.Invoke(); 
        });
    }

    public void OnDestory()
    {
        if(sequence != null && sequence.IsActive())
            sequence.Kill(); 

        OnCardMoved = null;
        OnCardMoveComplete = null; 
    }
}
