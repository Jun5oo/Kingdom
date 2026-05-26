using DG.Tweening;
using System;
using TMPro;
using UnityEngine;


/// <summary>
/// 데미지 수치를 팝업으로 표시하는 UI 컴포넌트. IPoolable로 풀에서 관리된다.
/// Play() 호출 시 확대(0.5s) → 대기(1.5s) → 페이드아웃(0.5s) 후 onCompleteCallback을 실행하고 풀로 반환된다.
/// </summary>
public class DamagePopup : MonoBehaviour, IPoolable
{
    [SerializeField] TextMeshProUGUI damage;

    Sequence sequence; 

    public void Init(int damage)
    {
        this.damage.text = damage.ToString();
        sequence = DOTween.Sequence();

        ResetSettings(); 
    }

    public void Play(Action onCompleteCallback)
    {
        this.gameObject.SetActive(true);
        sequence.Append(damage.transform.DOScale(1.3f, 0.5f).SetEase(Ease.OutBack))
            .AppendInterval(1.5f)
            .Append(damage.DOFade(0f, 0.5f).SetEase(Ease.InOutSine))
            .OnComplete(() =>
            {
                this.gameObject.SetActive(false);
                onCompleteCallback?.Invoke();
                ResetSettings();
            }
        );
    }

    public void ResetSettings()
    {
        damage.transform.localScale = Vector3.one * 0.5f;
        
        Color color = damage.color;
        color.a = 1f;

        damage.color = color; 
    }

}
