using DG.Tweening;
using System;
using UnityEngine;

/// <summary>
/// 카드 오브젝트의 이동을 담당하는 클래스
/// </summary>

public class CardMovement : MonoBehaviour
{
    [SerializeField] public PRS prs;
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

    Sequence sequence; 

    public void MoveTransform(PRS targetPRS, float duration, bool isHover = false, Action callback = null)
    {
        if (!isHover)
            PRS = targetPRS;

        isMoving = true;

        sequence = DOTween.Sequence(); 
        sequence.Append(transform.DOMove(targetPRS.position, duration));
        sequence.Join(transform.DORotateQuaternion(targetPRS.rotation, duration));
        sequence.Join(transform.DOScale(targetPRS.scale, duration));

        sequence.OnComplete(() =>
        {
            isMoving = false;
            callback?.Invoke();
            OnCardMovedComplete?.Invoke();
        });
    }
    public void AttackTargetFrom(Vector3 target, PRS from, Action onHitCallback = null, Action onCompleteCallback = null)
    {
        isMoving = true;

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
            isMoving = false;
            onCompleteCallback?.Invoke();
            OnCardMovedComplete?.Invoke();
        });
    }

    public void OnDestory()
    {
        if(sequence != null && sequence.IsActive())
            sequence.Kill(); 

        OnCardMoved = null;
        OnCardMovedComplete = null;
    }
}
