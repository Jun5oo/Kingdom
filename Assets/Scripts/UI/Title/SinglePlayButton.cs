using UnityEngine;

public class SinglePlayButton : MonoBehaviour
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
