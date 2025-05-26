using DG.Tweening;
using System;
using System.Runtime.InteropServices.WindowsRuntime;
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

    // 카드가 움직이기 시작했을 때 
    public Action<PRS> OnCardMoved;
    // 카드가 움직임을 끝냈을 때 
    public Action OnCardMovedComplete;

    bool isMoving = false;

    public bool IsMoving() => isMoving;

    public void MoveTransform(PRS targetPRS, float duration, bool isHover = false, Action callback = null)
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

        sequence.OnComplete(() =>
        {
            isMoving = false;
            callback?.Invoke();
            OnCardMovedComplete?.Invoke();
        }); ;
    }
}
