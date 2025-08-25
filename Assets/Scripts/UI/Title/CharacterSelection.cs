using UnityEngine;

public class CharacterSelection : MonoBehaviour
{
    [SerializeField] GameObject characterSelectionPanel;

    void Awake()
    {
        characterSelectionPanel.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.Escape))
            characterSelectionPanel.SetActive(false);
    }
}
