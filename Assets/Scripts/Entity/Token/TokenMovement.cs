using DG.Tweening;
using System;
using UnityEngine;

public class TokenMovement : MonoBehaviour
{
    const int HEIGHT = 5; 

    PRS prs;
    public PRS PRS
    {
        get { return prs; }
        set 
        { 
            prs = value;
            OnTokenMoved?.Invoke(); 
        } 
    }

    public Action OnTokenMoved;
    public Action OnTokenMoveComplete; 

    Sequence sequence;

    public void Init()
    {
        Vector3 position = Vector3.zero;
        Vector3 eulerAngles = new Vector3(90f, 0f, 0f);
        Quaternion quaternion = Quaternion.Euler(eulerAngles);
        Vector3 scale = Vector3.one;

        PRS = new PRS(position, quaternion, scale); 
    }

    public void MoveTransform(PRS targetPRS, float duration, bool isHover = false, Action callback = null)
    {
        if(!isHover)
            PRS = targetPRS;

        sequence = DOTween.Sequence();
        sequence.Append(transform.DOMove(targetPRS.position, duration));
        sequence.Join(transform.DORotateQuaternion(targetPRS.rotation, duration));
        sequence.Join(transform.DOScale(targetPRS.scale, duration));

        sequence.OnComplete(() =>
        {
            callback?.Invoke();
            OnTokenMoveComplete?.Invoke(); 
        });
    }
    public void AttackTargetFrom(Vector3 target, PRS from, Action onHitCallback = null, Action onCompleteCallback = null)
    {
        Sequence sequence = DOTween.Sequence();

        Vector3 distance = (target - from.position).normalized;

        Quaternion quaternion = Quaternion.LookRotation(-Vector3.up, distance);

        sequence.Append(transform.DORotateQuaternion(quaternion, 0.5f));
        sequence.Join(transform.DOMove(from.position + Vector3.up * 5, 0.5f));

        sequence.Append(transform.DOMove(target, 0.1f).SetEase(Ease.InQuart));
        sequence.AppendCallback(() => { onHitCallback?.Invoke(); });

        sequence.Append(transform.DOMove(from.position + Vector3.up * 5, 0.5f));
        sequence.Append(transform.DORotateQuaternion(from.rotation, 0.5f));
        sequence.Join(transform.DOMove(from.position, 0.5f));

        sequence.OnComplete(() =>
        {
            onCompleteCallback?.Invoke();
        });
    }
    public void PlayerSpinToss(Action onPeakCallback = null, Action onCompleteCallback = null)
    {
        Vector3 startPosition = PRS.position;
        Vector3 peakPosition = startPosition + Vector3.up * HEIGHT;

        transform.DORotate(new Vector3(1800f, 0f, 0f), 1f, RotateMode.LocalAxisAdd);
        
        sequence = DOTween.Sequence();
        sequence.Append(transform.DOMove(peakPosition, 1.0f).SetEase(Ease.OutQuad));

        sequence.AppendCallback(() => { onPeakCallback?.Invoke(); });

        sequence.Append(transform.DOMove(startPosition, 1.0f).SetEase(Ease.InQuad));
        sequence.OnComplete(() =>
        {
            onCompleteCallback?.Invoke();
        }); 
    }
}
