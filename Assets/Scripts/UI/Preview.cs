using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Preview : MonoBehaviour
{
    [SerializeField] RawImage art;
    [SerializeField] RawImage background;
    [SerializeField] RawImage frame; 

    [SerializeField] TextMeshProUGUI cardName;
    [SerializeField] TextMeshProUGUI description;
    [SerializeField] TextMeshProUGUI cp;
    [SerializeField] TextMeshProUGUI movement;

    [SerializeField] RectTransform viewPort; 

    public void OnUpdate(BaseObject baseObject, VisualTexture textures)
    {
        art.texture = textures.Art;
        background.texture = textures.Background; 
        frame.texture = textures.Frame;

        cardName.text = baseObject.Name;
        
        description.enableAutoSizing = false;

        description.text = baseObject.Description;
        
        float width = Mathf.Max(1f, viewPort.rect.width);
        float preferredHeight = description.GetPreferredValues(description.text, width, 0f).y; 

        if (viewPort.rect.height < preferredHeight)
            TopAlignment();
        else
            CenterAlignment(); 

        if(baseObject is Token token)
        {
            cp.enabled = true;
            movement.enabled = true; 

            cp.text = token.CP.ToString();
            movement.text = token.Movement.ToString();
        }
        else
        {
            cp.enabled = false; 
            movement.enabled = false; 
        }
    }

    void CenterAlignment()
    {
        var rt = description.rectTransform;
        rt.anchorMin= new Vector2(rt.anchorMin.x, 0.5f);
        rt.anchorMax = new Vector2(rt.anchorMax.x, 0.5f);
        rt.pivot = new Vector2(rt.pivot.x, 0.5f);
        rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, 0f); 
    }

    void TopAlignment()
    {
        var rt = description.rectTransform;
        rt.anchorMin = new Vector2(rt.anchorMin.x, 1f);
        rt.anchorMax = new Vector2(rt.anchorMax.x, 1f);
        rt.pivot = new Vector2(rt.pivot.x, 1f);
        rt.anchoredPosition = new Vector2(rt.anchoredPosition.x, 0f);
    }
}
