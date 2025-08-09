using UnityEngine;

public class CardView : BaseView
{
    [SerializeField] MeshRenderer mr; 
    [SerializeField] Transform anchorPosition; 
    public override Transform Anchor { get { return anchorPosition; } }

    public void Init(VisualTexture textures)
    {
        Material cardMaterial = mr.GetComponent<Renderer>().material;

        cardMaterial.SetTexture("_CardArtTexture", textures.Art);
        cardMaterial.SetTexture("_MaskTexture", textures.Mask);
        cardMaterial.SetTexture("_CardFrameTexture", textures.Frame);
        cardMaterial.SetTexture("_BackgroundTexture", textures.Background);
        cardMaterial.SetTexture("_BackTexture", textures.Back);
    }
}
