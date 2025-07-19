using DG.Tweening;
using System;
using UnityEngine;

public abstract class EntityMovement : MonoBehaviour
{
    public PRS prs;
    public PRS PRS
    {
        get { return prs; }
        set
        {
            prs = value;
            OnMoved?.Invoke();
        }
    }

    public Action OnMoved;
    public Action OnMovedComplete;

    public Sequence sequence;

    public abstract void Init();

    public void MoveTransform(PRS targetPRS, float duration, bool isHover = false, Action callback = null)
    {
        if (!isHover)
            PRS = targetPRS;

        OnMoved?.Invoke();

        sequence = DOTween.Sequence();
        sequence.Append(transform.DOMove(targetPRS.position, duration));
        sequence.Join(transform.DORotateQuaternion(targetPRS.rotation, duration));
        sequence.Join(transform.DOScale(targetPRS.scale, duration));

        sequence.OnComplete(() =>
        {
            callback?.Invoke();
            OnMovedComplete?.Invoke();
        });
    }

    void OnDestroy()
    {
        if (sequence != null && sequence.IsActive())
            sequence.Kill();

        OnMoved = null;
        OnMovedComplete = null;
    }
}
