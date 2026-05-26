using DG.Tweening;
using System;
using UnityEngine;

/// <summary>
/// DOTween 기반 Transform 이동 애니메이션의 공통 기반 클래스.
/// MoveTransform()으로 위치·회전·크기를 동시에 트위닝하며,
/// isHover=true이면 PRS 상태를 갱신하지 않아 일시적인 시각 효과에 사용할 수 있다.
/// </summary>
public abstract class BaseMovement : MonoBehaviour
{
    protected PRS prs; // 현재 논리적 위치·회전·크기 상태

    /// <summary> PRS를 설정할 때 OnMoved 이벤트를 발생시킨다. </summary>
    public PRS PRS
    {
        get { return prs; }
        set
        {
            prs = value;
            OnMoved?.Invoke();
        }
    }

    public Action OnMoved;         // PRS가 변경되었을 때 발생
    public Action OnMovedComplete; // 트위닝 애니메이션이 완료되었을 때 발생

    public Sequence sequence;

    public abstract void Init();

    /// <summary>
    /// 목표 PRS로 duration 초 동안 이동 애니메이션을 실행한다.
    /// isHover=false(기본)이면 PRS 상태도 갱신한다.
    /// 완료 시 callback과 OnMovedComplete를 순서대로 호출한다.
    /// </summary>
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
