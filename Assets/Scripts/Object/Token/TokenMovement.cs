using DG.Tweening;
using System;
using UnityEngine;

/// <summary>
/// 토큰의 이동 및 공격 애니메이션을 처리하는 컴포넌트.
/// BaseMovement를 상속하며, AttackAnimationController에 근접/원거리 공격 애니메이션을 위임한다.
/// </summary>
public class TokenMovement : BaseMovement
{
    const int HEIGHT = 5;

    [Header("Attack Animation")]
    [SerializeField] AttackAnimationController attackAnimationController;

    /// <summary> 초기 PRS를 (0, 90도 X 회전, 스케일 1)로 설정한다. </summary>
    public override void Init()
    {
        Vector3 position = Vector3.zero;
        Vector3 eulerAngles = new Vector3(90f, 0f, 0f);
        Quaternion quaternion = Quaternion.Euler(eulerAngles);
        Vector3 scale = Vector3.one;

        PRS = new PRS(position, quaternion, scale);
    }

    /// <summary>
    /// 근접 공격 애니메이션을 실행한다.
    /// onHitCallback: 타격 순간 호출 (데미지 처리), onCompleteCallback: 전체 완료 후 호출.
    /// AttackAnimationController가 없으면 즉시 콜백을 호출한다.
    /// </summary>
    public void MeleeAttackTargetFrom(Vector3 target, PRS from, Action onHitCallback = null, Action onCompleteCallback = null)
    {
        if (attackAnimationController != null)
        {
            attackAnimationController.MeleeAttackAnimation(target, from, onHitCallback, onCompleteCallback);
        }
        else
        {
            Debug.LogWarning("AttackAnimationController is not assigned!");
            onHitCallback?.Invoke();
            onCompleteCallback?.Invoke();
        }
    }

    /// <summary>
    /// 원거리 공격 애니메이션을 실행한다.
    /// onHitCallback: 투사체가 목표에 닿는 순간 호출, onCompleteCallback: 전체 완료 후 호출.
    /// </summary>
    public void RangeAttackTargetFrom(Vector3 target, PRS from, Action onHitCallback = null, Action onCompleteCallback = null)
    {
        if (attackAnimationController != null)
        {
            attackAnimationController.RangeAttackAnimation(target, from, onHitCallback, onCompleteCallback);
        }
        else
        {
            Debug.LogWarning("AttackAnimationController is not assigned!");
            onHitCallback?.Invoke();
            onCompleteCallback?.Invoke();
        }
    }

    /// <summary> MeleeAttackTargetFrom의 별칭 (하위 호환성 유지용). </summary>
    public void AttackTargetFrom(Vector3 target, PRS from, Action onHitCallback = null, Action onCompleteCallback = null)
    {
        MeleeAttackTargetFrom(target, from, onHitCallback, onCompleteCallback);
    }
}
