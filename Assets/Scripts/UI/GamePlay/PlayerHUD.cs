using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 플레이어 정보(킹 초상화, 이름, CP 슬라이더)를 표시하는 HUD.
/// Init() 시 킹 토큰의 OnCPUpdate 이벤트를 구독해 실시간으로 슬라이더를 갱신한다.
/// </summary>
public class PlayerHUD : MonoBehaviour
{
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

        Texture2D texture = await loader.LoadArtAsync(token.Data.ID);

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
