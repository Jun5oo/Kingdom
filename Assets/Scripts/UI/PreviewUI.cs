using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PreviewUI : MonoBehaviour
{
    [SerializeField] Image image;
    [SerializeField] TextMeshProUGUI cardName;
    [SerializeField] TextMeshProUGUI description;
    [SerializeField] TextMeshProUGUI level;
    [SerializeField] TextMeshProUGUI cp;
    [SerializeField] TextMeshProUGUI movement; 

    public void OnUpdate(Entity entity)
    {
        image.sprite = entity.Sprite;
        cardName.text = entity.Name;
        description.text = entity.Description;
        level.text = entity.Level.ToString();
        cp.text = entity.CP.ToString(); 
        movement.text = entity.Movement.ToString();
    }
}
