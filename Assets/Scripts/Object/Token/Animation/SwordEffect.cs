using UnityEngine;

/// <summary>
/// 칼 이펙트 프리팹 컴포넌트. Start 시 지정 색상을 적용하고 lifetime 이후 자동 파괴된다.
/// TrailRenderer는 0.1초 지연 후 활성화하여 생성 직후 잔상을 방지한다.
/// </summary>
public class SwordEffect : MonoBehaviour
{
    [Header("Sword Settings")]
    [SerializeField] float lifetime = 0.5f;
    [SerializeField] Color swordColor = Color.white;
    [SerializeField] float swordScale = 1f;
    
    [Header("Visual Effects")]
    [SerializeField] SpriteRenderer swordRenderer;
    [SerializeField] TrailRenderer trailRenderer;
    
    void Start()
    {
        // 칼 스프라이트 설정
        if (swordRenderer == null)
            swordRenderer = GetComponent<SpriteRenderer>();
            
        if (swordRenderer != null)
        {
            swordRenderer.color = swordColor;
            transform.localScale = Vector3.one * swordScale;
        }
        
        // 트레일 효과 설정
        if (trailRenderer == null)
            trailRenderer = GetComponent<TrailRenderer>();
            
        if (trailRenderer != null)
        {
            trailRenderer.startColor = swordColor;
            trailRenderer.endColor = new Color(swordColor.r, swordColor.g, swordColor.b, 0f);
            
            // 트레일 렌더러를 처음에는 비활성화
            trailRenderer.enabled = false;
            
            // 0.1초 후에 트레일 렌더러 활성화
            Invoke(nameof(EnableTrailRenderer), 0.1f);
        }
        
        // 자동 제거
        Destroy(gameObject, lifetime);
    }
    
    // 트레일 렌더러를 활성화하는 메서드
    private void EnableTrailRenderer()
    {
        if (trailRenderer != null)
        {
            trailRenderer.enabled = true;
        }
    }
    
    // 칼 휘두르기 효과
    public void SwingEffect()
    {
        // 파티클 효과나 사운드 효과를 여기에 추가할 수 있습니다
        // 예: 파티클 시스템, 사운드 재생 등
    }
} 