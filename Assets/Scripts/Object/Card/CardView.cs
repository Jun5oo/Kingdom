using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// 카드의 시각 표현을 담당한다.
/// 머티리얼 텍스처(아트/마스크/프레임/배경/뒷면) 설정과 CP·이동력 텍스트 표시를 관리한다.
/// </summary>
public class CardView : BaseView
{
    [SerializeField] MeshRenderer mr;
    [SerializeField] TextMeshProUGUI cp;
    [SerializeField] TextMeshProUGUI movement;

    [SerializeField] Transform anchorPosition; // 팝업 표시 기준점

    public override Transform Anchor { get { return anchorPosition; } }

    /// <summary> 텍스처를 적용하고 레벨 1 기준의 CP/이동력을 표시한다. </summary>
    public void Init(VisualTexture textures, CardData cardData)
    {
        SetCardView(textures.Art, textures.Mask, textures.Frame, textures.Background, textures.Back);

        SetCardCP(cardData.CP[0]);
        SetCardMovement(cardData.MoveRange[0]);
    }

    /// <summary> 카드 머티리얼의 5개 텍스처 슬롯을 교체한다. </summary>
    public void SetCardView(Texture2D art, Texture2D mask, Texture2D frame, Texture2D background, Texture2D back)
    {
        Material cardMaterial = mr.GetComponent<Renderer>().material;

        cardMaterial.SetTexture("_CardArtTexture", art);
        cardMaterial.SetTexture("_MaskTexture", mask);
        cardMaterial.SetTexture("_CardFrameTexture", frame);
        cardMaterial.SetTexture("_BackgroundTexture", background);
        cardMaterial.SetTexture("_BackTexture", back);
    }

    public void SetCardCP(int cp) => this.cp.text = cp.ToString();
    public void SetCardMovement(int movement) => this.movement.text = movement.ToString();
}
