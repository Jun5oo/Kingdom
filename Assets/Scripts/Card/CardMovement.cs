using DG.Tweening;
using UnityEngine;

public class CardMovement : MonoBehaviour
{
    public PRS prs;
    public bool isMoving = false;

    public void MoveTransform(PRS targetPRS, float duration)
    {
        prs = targetPRS;

        isMoving = true;

        Sequence sequence = DOTween.Sequence();

        sequence.Append(transform.DOMove(prs.position, duration));
        sequence.Join(transform.DORotateQuaternion(prs.rotation, duration));
        sequence.Join(transform.DOScale(prs.scale, duration));

        sequence.OnComplete(() => isMoving = false);
    }
}
