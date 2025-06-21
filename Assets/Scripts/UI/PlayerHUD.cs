using TMPro;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{
    const int PLAYER_POS_X = 190;
    const int ENEMY_POS_X = -190;
    const int POS_Y = 0; 

    [SerializeField] RectTransform panel; 

    [SerializeField] Image playerImage;
    [SerializeField] TextMeshProUGUI playerName;
    [SerializeField] TextMeshProUGUI playerCp;

    [SerializeField] Sprite angel;
    [SerializeField] Sprite undead; 

    public void Init(bool isLocal, int cp)
    {
        if (isLocal)
        {
            panel.anchoredPosition = new Vector2(PLAYER_POS_X, POS_Y);
            playerName.text = "Player1";
            playerImage.sprite = undead;  
        }

        else
        {
            panel.anchoredPosition = new Vector2(ENEMY_POS_X, POS_Y);
            playerName.text = "Player2";
            playerImage.sprite = angel;
        }

        playerCp.text = cp.ToString();
    }

    public void OnUpdateCP(int cp)
    {
        playerCp.text = cp.ToString(); 
    }
}
