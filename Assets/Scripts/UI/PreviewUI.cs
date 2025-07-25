using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PreviewUI : MonoBehaviour
{
    [SerializeField] RawImage art;
    [SerializeField] RawImage background;
    [SerializeField] RawImage frame; 

    [SerializeField] TextMeshProUGUI cardName;
    [SerializeField] TextMeshProUGUI description;
    [SerializeField] TextMeshProUGUI cp;
    [SerializeField] TextMeshProUGUI movement;

    public void OnUpdate(BaseObject baseObject, VisualTexture textures)
    {
        art.texture = textures.Art;
        background.texture = textures.Background; 
        frame.texture = textures.Frame;

        cardName.text = baseObject.Name;
        description.text = baseObject.Description;

        if(baseObject.Data is UnitCardData data)
        {
            cp.enabled = true;
            movement.enabled = true; 

            cp.text = data.CP.ToString();
            movement.text = data.Movement.ToString();
        }
        else
        {
            cp.enabled = false; 
            movement.enabled = false; 
        }
    }
}
