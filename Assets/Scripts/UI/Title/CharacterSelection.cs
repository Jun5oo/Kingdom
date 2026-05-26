using UnityEngine;

/// <summary>
/// 진영 선택 패널 표시 컴포넌트. Awake 시 패널을 숨기고 ESC 키로 닫는다.
/// </summary>
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
