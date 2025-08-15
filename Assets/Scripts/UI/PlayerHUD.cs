using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{
    const string undeadID = "1";
    const string celestialID = "4"; 

    [SerializeField] RectTransform panel; 

    [SerializeField] RawImage playerImage;
    [SerializeField] TextMeshProUGUI playerName;
    [SerializeField] TextMeshProUGUI playerCp;

    CardTextureLoader loader; 

    public async UniTask Init(Player player, Token token)
    {
        loader = ServiceLocator.Get<CardTextureLoader>();

        token.OnCPUpdate -= OnUpdateCP; 
        token.OnCPUpdate += OnUpdateCP;

        string cardID = null; 

        switch (player.Race)
        {
            case Race.Undead:
                cardID = undeadID; 
                break;
            case Race.Celestial:
                cardID = celestialID;
                break;
            default:
                Debug.Log("아직 구현되지 않은 진영입니다.");
                return; 
        }

        Texture2D texture = await loader.LoadArtAsync(cardID);

        float aspect = (float)texture.width / texture.height;

        if (playerImage.gameObject.TryGetComponent<AspectRatioFitter>(out AspectRatioFitter ratioFitter))
            ratioFitter.aspectRatio = aspect; 

        playerImage.texture = texture; 
        playerName.text = player.PlayerName;
        playerCp.text = token.CP.ToString(); 
    }

    void OnUpdateCP(int cp)
    {
        playerCp.text = cp.ToString(); 
    }
}
