using DG.Tweening;
using System;
using TMPro;
using UnityEngine;


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
