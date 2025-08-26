using DG.Tweening;
using System;
using UnityEngine;

public class TokenMovement : BaseMovement
{
    const int HEIGHT = 5; 

    [Header("Attack Animation")]
    [SerializeField] AttackAnimationController attackAnimationController; // 공격 애니메이션 컨트롤러

    public override void Init()
    {
        Vector3 position = Vector3.zero;
        Vector3 eulerAngles = new Vector3(90f, 0f, 0f);
        Quaternion quaternion = Quaternion.Euler(eulerAngles);
        Vector3 scale = Vector3.one;

        PRS = new PRS(position, quaternion, scale); 
    }

    // 근접 공격 애니메이션 (칼 휘두르기)
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

    // 원거리 공격 애니메이션 (활과 화살)
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

    // 기존 메서드 (하위 호환성을 위해 유지)
    public void AttackTargetFrom(Vector3 target, PRS from, Action onHitCallback = null, Action onCompleteCallback = null)
    {
        MeleeAttackTargetFrom(target, from, onHitCallback, onCompleteCallback);
    }
}
