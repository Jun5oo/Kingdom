using UnityEngine;
using UnityEngine.Rendering;

public class CardView : EntityView
{
    [SerializeField] MeshRenderer mr; 
    [SerializeField] Transform anchorUIPosition; 
    public override Transform AnchorUI { get { return anchorUIPosition; } }

    public void Init(Texture2D texture)
    {
        Material cardMaterial = null;
        cardMaterial = mr.GetComponent<Renderer>().material;
        cardMaterial.SetTexture("_CardArtTexture", texture);
    }
}
