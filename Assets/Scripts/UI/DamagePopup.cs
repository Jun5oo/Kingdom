using DG.Tweening;
using System;
using TMPro;
using UnityEngine;


public class DamagePopup : MonoBehaviour, IPoolable
{
    [SerializeField] TextMeshProUGUI damage;

    public void Init(int damage)
    {
        this.damage.text = damage.ToString();
    }

    public void Play(Action onCompleteCallback)
    {
        this.gameObject.SetActive(true);

        damage.DOFade(1f, 1f).SetEase(Ease.InOutSine).OnComplete(() =>
        {
            damage.DOFade(0f, 1f).SetDelay(1f);
            this.gameObject.SetActive(false);
            onCompleteCallback?.Invoke(); 
        });
    }

}
