using UnityEngine;

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
        }
        
        // 자동 제거
        Destroy(gameObject, lifetime);
    }
    
    // 칼 휘두르기 효과
    public void SwingEffect()
    {
        // 파티클 효과나 사운드 효과를 여기에 추가할 수 있습니다
        // 예: 파티클 시스템, 사운드 재생 등
    }
} 