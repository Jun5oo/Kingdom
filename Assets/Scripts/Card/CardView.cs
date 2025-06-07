using TMPro;
using UnityEngine;

/// <summary>
/// 카드 오브젝트의 View를 처리하는 클래스
/// </summary>

public class CardView : MonoBehaviour
{
    IUISystem uiSystem; 
    Card card;

    [SerializeField] SpriteRenderer sr; 
    // [SerializeField] TextMeshPro nameTMP;

    public void Init(IUISystem uiSystem, Card card)
    {
        this.uiSystem = uiSystem;
        this.card = card;

        sr.sprite = card.Image; 
        // nameTMP.text = card.Name;
    }

    [SerializeField] GameObject cardStatusUI;

    public void DisplayStatusUI()
    {
        cardStatusUI = uiSystem.Pop<CardStatusUI>();
        cardStatusUI.GetComponent<CardStatusUI>().OnUpdate(card.CP, card.Movement); 
        cardStatusUI.transform.position = Camera.main.WorldToScreenPoint(transform.position);
    }

    public void HideStatusUI()
    {
        if (cardStatusUI == null)
            return; 

        uiSystem.Push<CardStatusUI>(cardStatusUI);
        cardStatusUI = null; 
    }

    public void UpdateStatusUI()
    {
        if(cardStatusUI != null)
            cardStatusUI.GetComponent<CardStatusUI>().OnUpdate(card.CP, card.Movement); 
    }
}
