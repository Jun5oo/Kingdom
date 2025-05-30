using TMPro;
using Unity.VisualScripting;
using UnityEngine;

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

    GameObject cardStatusUI;

    public void DisplayStatusUI()
    {
        // Temp 
        UISystem uiSystem = GameObject.FindAnyObjectByType<UISystem>();
        cardStatusUI = uiSystem.Pop<CardStatusUI>();
        cardStatusUI.GetComponent<CardStatusUI>().OnUpdate(card.Cp, card.Movement); 
        cardStatusUI.SetActive(true); 
        cardStatusUI.transform.position = Camera.main.WorldToScreenPoint(transform.position);
        // 
    }

    public void HideStatusUI()
    {
        // Temp 
        cardStatusUI.SetActive(true);
        UISystem uiSystem = GameObject.FindAnyObjectByType<UISystem>();
        uiSystem.Push<CardStatusUI>(cardStatusUI);
        cardStatusUI = null; 
        // 
    }

    public void UpdateStatusUI()
    {
        if (cardStatusUI == null)
            return;
    }
}
