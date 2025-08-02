using TMPro;
using UnityEngine;

public class TokenView : BaseView
{
    [SerializeField] MeshRenderer mr;
    [SerializeField] TextMeshPro cp;
    [SerializeField] TextMeshPro movement;
    [SerializeField] Transform anchorPosition;

    public override Transform Anchor {  get { return anchorPosition; } }

    public void Init(VisualTexture textures, int cp, int movement)
    {
        SetTokenView(textures.Art, textures.Frame, textures.Mask); 
        
        this.cp.text = cp.ToString();
        this.movement.text = movement.ToString();
    }

    public void OnUpdateCP(int cp)
    {
        this.cp.text = cp.ToString(); 
    }

    public void OnUpdateMovement(int movement)
    {
        this.movement.text = movement.ToString(); 
    }

    public void SetTokenView(Texture2D art, Texture2D frame, Texture2D artMask)
    {
        Material tokenMaterial = mr.GetComponent<Renderer>().material;
        tokenMaterial.SetTexture("_TokenArt", art);
        tokenMaterial.SetTexture("_TokenFrame", frame);
        tokenMaterial.SetTexture("_TokenFrameMask", artMask); 
    }
}
