using UnityEngine;
using UnityEngine.EventSystems;

public class SinglePlay : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] GameObject selectionPanel; 

    public void ActivePanel()
    {
        if (selectionPanel != null) 
        {
            selectionPanel.SetActive(true);
        }
    }

    public void OnPointerEnter(PointerEventData eventData) => transform.localScale = Vector3.one * 1.1f;
    public void OnPointerExit(PointerEventData eventData) => transform.localScale = Vector3.one; 
}
