using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PreviewUI : MonoBehaviour
{
    [SerializeField] Image image;
    [SerializeField] TextMeshProUGUI cardName;
    [SerializeField] TextMeshProUGUI description;
    [SerializeField] TextMeshProUGUI cp;
    [SerializeField] TextMeshProUGUI movement; 

    public void OnUpdate(Entity entity)
    {
        image.sprite = entity.Sprite;
        cardName.text = entity.Name;
        description.text = entity.Description;

        if(entity is IUnit unit)
        {
            movement.text = unit.Movement.ToString();
            cp.text = unit.CP.ToString();
        }
    }
}
