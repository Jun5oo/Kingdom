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

    public void OnUpdate(BaseObject baseObject, VisualTexture textures)
    {
        art.texture = textures.Art;
        background.texture = textures.Background; 
        frame.texture = textures.Frame;

        cardName.text = baseObject.Name;
        description.text = baseObject.Description;

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
}
