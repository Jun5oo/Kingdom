using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class CardView : BaseView
{
    [SerializeField] MeshRenderer mr;
    [SerializeField] TextMeshProUGUI cp;
    [SerializeField] TextMeshProUGUI movement;

    [SerializeField] Transform anchorPosition;

    public override Transform Anchor { get { return anchorPosition; } }

    public void Init(VisualTexture textures, CardData cardData)
    {
        SetCardView(textures.Art, textures.Mask, textures.Frame, textures.Background, textures.Back);

        // 현재 카드의 경우 레벨 1이라고 가정 
        SetCardCP(cardData.CP[0]);
        SetCardMovement(cardData.MoveRange[0]);
    }

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
