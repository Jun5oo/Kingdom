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

        UnitCardData unitData;

        if (cardData is UnitCardData)
            unitData = cardData as UnitCardData;
        else
            return;

        SetCardCP(unitData.CP[unitData.Level-1]);
        SetCardMovement(unitData.Movement[unitData.Level-1]); 
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
