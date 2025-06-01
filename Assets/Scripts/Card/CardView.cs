using TMPro;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 카드 오브젝트의 View를 처리하는 클래스
/// </summary>

public class CardView : MonoBehaviour
{
    Card card; 

    [SerializeField] Renderer rd; 
    [SerializeField] TextMeshPro nameTMP;
    [SerializeField] TextMeshPro descriptionTMP;

    public void Init(Card card)
    {
        this.card = card; 

        rd.material.mainTexture = card.Image.texture;
        nameTMP.text = card.Name;
        descriptionTMP.text = card.Description; 
    }

    [SerializeField] GameObject cardStatusUI;

    public void DisplayStatusUI()
    {
        // Temp (현재는 직접적으로 찾아오지만, 초기화 시 IUISystem을 주입해줄 예정 
        UISystem uiSystem = GameObject.FindAnyObjectByType<UISystem>();
        cardStatusUI = uiSystem.Pop<CardStatusUI>();
        cardStatusUI.GetComponent<CardStatusUI>().OnUpdate(card.Cp, card.Movement); 
        cardStatusUI.SetActive(true); 
        cardStatusUI.transform.position = Camera.main.WorldToScreenPoint(transform.position);
    }

    public void HideStatusUI()
    {
        if (cardStatusUI == null)
            return; 

        cardStatusUI.SetActive(true);

        // Temp
        UISystem uiSystem = GameObject.FindAnyObjectByType<UISystem>();
        uiSystem.Push<CardStatusUI>(cardStatusUI);
        cardStatusUI = null; 
    }
    public void UpdateStatusUI()
    {
        if (cardStatusUI == null)
            return;
    }
}
