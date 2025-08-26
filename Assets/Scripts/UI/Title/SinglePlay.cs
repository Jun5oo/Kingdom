using UnityEngine;

public class SinglePlay : MonoBehaviour
{
    [SerializeField] GameObject selectionPanel; 

    public void ActivePanel()
    {
        if (selectionPanel != null) 
        {
            selectionPanel.SetActive(true);
        }
    }
}
