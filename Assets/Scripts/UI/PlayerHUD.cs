using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{
    [SerializeField] RectTransform panel; 

    [SerializeField] Image playerImage;
    [SerializeField] TextMeshProUGUI playerName;
    [SerializeField] TextMeshProUGUI playerCp;

    [SerializeField] Sprite angel;
    [SerializeField] Sprite undead; 

    public void Init(Player playerData, Token token)
    {
        token.OnCPUpdate -= OnUpdateCP; 
        token.OnCPUpdate += OnUpdateCP;

        // 나중에 종족별 King Image로 업데이트 
        // playerImage.sprite = null;

        playerName.text = playerData.PlayerName;
        playerCp.text = token.CP.ToString(); 
    }

    void OnUpdateCP(int cp)
    {
        playerCp.text = cp.ToString(); 
    }
}
