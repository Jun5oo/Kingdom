using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 카드 UI 클래스 (클릭했을 때 보이는 Display UI) 
/// </summary>

public class CardUI : MonoBehaviour
{
    [SerializeField] RawImage image;
    [SerializeField] TextMeshProUGUI cardName;
    [SerializeField] TextMeshProUGUI description;
    [SerializeField] TextMeshProUGUI level;
    [SerializeField] TextMeshProUGUI cp;

    public void OnUpdate(Card card)
    {
        image.texture = card.Image.texture;
        cardName.text = card.Name;
        description.text = card.Description;
        level.text = card.Level.ToString();
        cp.text = card.Cp.ToString(); 
    }
}
