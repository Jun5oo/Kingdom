using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUD : MonoBehaviour
{
    const string undeadID = "1";
    const string celestialID = "4";
    const int MAX_CP = 40; 

    [SerializeField] RawImage playerImage;
    [SerializeField] TextMeshProUGUI playerName;
    [SerializeField] TextMeshProUGUI currentCP;
    [SerializeField] TextMeshProUGUI maxCP; 

    [SerializeField] Slider slider; 

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
        currentCP.text = token.CP.ToString();

        slider.value = 1; 
        maxCP.text = MAX_CP.ToString();
        OnUpdateCP(MAX_CP); 
    }

    void OnUpdateCP(int cp)
    {
        if (cp <= 0)
            cp = 0;

        currentCP.text = cp.ToString();
        slider.value = (float)cp / MAX_CP;
    }
}
