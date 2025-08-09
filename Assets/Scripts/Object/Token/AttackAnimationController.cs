using DG.Tweening;
using System;
using UnityEngine;

public class AttackAnimationController : MonoBehaviour
{
    [Header("Melee Attack Components")]
    [SerializeField] GameObject swordPrefab; // 칼 프리팹
    [SerializeField] Transform swordSpawnPoint; // 칼 생성 위치
    [SerializeField] float swordSwingDuration = 0.3f; // 칼 휘두르는 시간
    
    [Header("Ranged Attack Components")]
    [SerializeField] GameObject bowPrefab; // 활 프리팹
    [SerializeField] GameObject arrowPrefab; // 화살 프리팹
    [SerializeField] Transform bowSpawnPoint; // 활 생성 위치
    [SerializeField] Transform arrowSpawnPoint; // 화살 생성 위치
    [SerializeField] float bowDrawDuration = 0.5f; // 활 당기는 시간
    [SerializeField] float arrowFlightDuration = 0.3f; // 화살 날아가는 시간
    
    [Header("Test Settings")]
    [SerializeField] bool enableTestMode = true; // 테스트 모드 활성화
    [SerializeField] Vector3 testTargetPosition = new Vector3(2, 0, 2); // 테스트용 타겟 위치
    [SerializeField] bool testRangedAttack = false; // 원거리 공격 테스트 모드

    void Update()
    {
        // 테스트 모드에서 K키를 누르면 공격 테스트
        if (enableTestMode && Input.GetKeyDown(KeyCode.K))
        {
            if (testRangedAttack)
            {
                TestRangedAttack();
            }
            else
            {
                TestSwordSwing();
            }
        }
    }

    // 테스트용 칼 휘두르기
    void TestSwordSwing()
    {
        Debug.Log("테스트: 칼 휘두르기 애니메이션 실행");
        
        Vector3 testTarget = transform.position + testTargetPosition;
        PRS currentPRS = new PRS(transform.position, transform.rotation, transform.localScale);
        
        MeleeAttackAnimation(testTarget, currentPRS, 
            onHitCallback: () => {
                Debug.Log("칼 휘두르기 히트!");
            },
            onCompleteCallback: () => {
                Debug.Log("칼 휘두르기 완료!");
            });
    }

    // 테스트용 원거리 공격
    void TestRangedAttack()
    {
        Debug.Log("테스트: 원거리 공격 애니메이션 실행");
        
        Vector3 testTarget = transform.position + testTargetPosition;
        PRS currentPRS = new PRS(transform.position, transform.rotation, transform.localScale);
        
        RangeAttackAnimation(testTarget, currentPRS, 
            onHitCallback: () => {
                Debug.Log("화살 히트!");
            },
            onCompleteCallback: () => {
                Debug.Log("원거리 공격 완료!");
            });
    }

    // 근접 공격 애니메이션 (칼 휘두르기)
    public void MeleeAttackAnimation(Vector3 target, PRS from, Action onHitCallback = null, Action onCompleteCallback = null)
    {
        Sequence sequence = DOTween.Sequence();

        Vector3 distance = (target - from.position).normalized;
        Quaternion quaternion = Quaternion.LookRotation(-Vector3.up, distance);

        // 1단계: 타겟을 향해 회전
        sequence.Append(transform.DORotateQuaternion(quaternion, 0.2f));
        sequence.Join(transform.DOMove(from.position + Vector3.up * 2, 0.2f));

        // 2단계: 칼 생성 및 휘두르기
        sequence.AppendCallback(() => {
            CreateAndSwingSword(target, onHitCallback, onCompleteCallback);
        });

        // 3단계: 원래 위치로 복귀 (칼 애니메이션 완료 후)
        sequence.Append(transform.DORotateQuaternion(from.rotation, 0.3f));
        sequence.Join(transform.DOMove(from.position, 0.3f));
    }

    // 칼 생성 및 휘두르기 애니메이션
    void CreateAndSwingSword(Vector3 target, Action onHitCallback, Action onCompleteCallback = null)
    {
        if (swordPrefab == null)
        {
            Debug.LogWarning("Sword prefab is not assigned!");
            onHitCallback?.Invoke();
            onCompleteCallback?.Invoke();
            return;
        }

        // 칼 생성 위치 계산 (자루 부분을 중심으로)
        Vector3 spawnPosition = swordSpawnPoint != null ? 
            swordSpawnPoint.position : transform.position + Vector3.up * 1f;

        // 칼 인스턴스 생성
        GameObject sword = Instantiate(swordPrefab, spawnPosition, Quaternion.identity);
        
        // 타겟 방향 계산
        Vector3 direction = (target - spawnPosition).normalized;
        float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

        // 칼을 눕힌 상태로 초기 회전 (X축 90도 회전으로 눕히기)
        sword.transform.rotation = Quaternion.Euler(90f, targetAngle, 0);

        // 칼 휘두르기 애니메이션
        Sequence swordSequence = DOTween.Sequence();
        
        // 칼 등장 효과
        swordSequence.Append(sword.transform.DOScale(Vector3.zero, 0f));
        swordSequence.Append(sword.transform.DOScale(Vector3.one, 0.1f).SetEase(Ease.OutBack));
        
        // 한 번에 확 휘두르기 (Z축 0도에서 -90도로)
        swordSequence.Append(sword.transform.DORotate(
            new Vector3(90f, targetAngle, -90f), 
            swordSwingDuration
        ).SetEase(Ease.OutQuart));

        // 공격 히트 시점
        swordSequence.AppendCallback(() => {
            // 히트 효과 (간단한 스케일 효과)
            sword.transform.DOScale(Vector3.one * 1.2f, 0.05f).SetLoops(2, LoopType.Yoyo);
            onHitCallback?.Invoke();
        });

        // 칼 사라짐 효과 (회수 없이 바로 사라짐)
        swordSequence.Append(sword.transform.DOScale(Vector3.zero, 0.1f).SetEase(Ease.InBack));

        // 칼 제거 및 데미지 처리
        swordSequence.OnComplete(() => {
            Destroy(sword);
            // 칼이 사라진 후 데미지 처리
            onCompleteCallback?.Invoke();
        });
    }

    // 원거리 공격 애니메이션 (활과 화살)
    public void RangeAttackAnimation(Vector3 target, PRS from, Action onHitCallback = null, Action onCompleteCallback = null)
    {
        Sequence sequence = DOTween.Sequence();

        Vector3 distance = (target - from.position).normalized;
        Quaternion quaternion = Quaternion.LookRotation(-Vector3.up, distance);

        // 1단계: 타겟을 향해 회전
        sequence.Append(transform.DORotateQuaternion(quaternion, 0.3f));
        sequence.Join(transform.DOMove(from.position + Vector3.up * 2, 0.3f));

        // 2단계: 활 생성 및 화살 발사
        sequence.AppendCallback(() => {
            CreateAndShootArrow(target, onHitCallback, onCompleteCallback);
        });

        // 3단계: 원래 위치로 복귀
        sequence.Append(transform.DORotateQuaternion(from.rotation, 0.4f));
        sequence.Join(transform.DOMove(from.position, 0.4f));
    }

    // 활 생성 및 화살 발사 애니메이션
    void CreateAndShootArrow(Vector3 target, Action onHitCallback, Action onCompleteCallback = null)
    {
        if (bowPrefab == null || arrowPrefab == null)
        {
            Debug.LogWarning("Bow or Arrow prefab is not assigned!");
            onHitCallback?.Invoke();
            onCompleteCallback?.Invoke();
            return;
        }

        // 활 생성 위치 계산
        Vector3 bowSpawnPosition = bowSpawnPoint != null ? 
            bowSpawnPoint.position : transform.position + Vector3.up * 1.5f;

        // 화살 생성 위치 계산
        Vector3 arrowSpawnPosition = arrowSpawnPoint != null ? 
            arrowSpawnPoint.position : transform.position + Vector3.up * 1.5f;

        // 타겟 방향 계산
        Vector3 direction = (target - arrowSpawnPosition).normalized;
        float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

        // 활 인스턴스 생성 (그냥 생성, 애니메이션 없음)
        GameObject bow = Instantiate(bowPrefab, bowSpawnPosition, Quaternion.identity);
        bow.transform.rotation = Quaternion.Euler(90f, targetAngle, -90f);

        // 활과 화살 애니메이션 시퀀스
        Sequence bowArrowSequence = DOTween.Sequence();
        
        // 활 등장 효과
        bowArrowSequence.Append(bow.transform.DOScale(Vector3.zero, 0f));
        bowArrowSequence.Append(bow.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack));
        
        // 활 등장 후 화살 생성
        bowArrowSequence.AppendCallback(() => {
            // 화살 인스턴스 생성
            GameObject arrow = Instantiate(arrowPrefab, arrowSpawnPosition, Quaternion.identity);
            arrow.transform.rotation = Quaternion.Euler(90f, targetAngle, 0);
            
            // 화살 애니메이션
            Sequence arrowSequence = DOTween.Sequence();
            
            // 화살 등장 효과
            arrowSequence.Append(arrow.transform.DOScale(Vector3.zero, 0f));
            arrowSequence.Append(arrow.transform.DOScale(Vector3.one, 0.1f).SetEase(Ease.OutBack));
            
            // 화살 직선 발사 (타겟으로 직선 이동)
            arrowSequence.Append(arrow.transform.DOMove(target, arrowFlightDuration).SetEase(Ease.OutQuart));
            
            // 화살 히트 시점
            arrowSequence.AppendCallback(() => {
                // 히트 효과 (화살 스케일 효과)
                arrow.transform.DOScale(Vector3.one * 1.3f, 0.05f).SetLoops(2, LoopType.Yoyo);
                onHitCallback?.Invoke();
            });
            
            // 화살 사라짐 효과
            arrowSequence.Append(arrow.transform.DOScale(Vector3.zero, 0.1f).SetEase(Ease.InBack));
            
            // 화살 제거 및 데미지 처리
            arrowSequence.OnComplete(() => {
                Destroy(arrow);
                Destroy(bow);
                // 화살이 사라진 후 데미지 처리
                onCompleteCallback?.Invoke();
            });
        });
    }
} 