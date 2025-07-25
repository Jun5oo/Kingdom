using TMPro;
using UnityEngine;

public class TokenView : BaseView
{
    [SerializeField] MeshRenderer mr;
    [SerializeField] TextMeshPro cp;
    [SerializeField] TextMeshPro movement;
    [SerializeField] Transform anchorPosition;

    VisualTexture textures; 
    public override Transform Anchor {  get { return anchorPosition; } }

    public void Init(VisualTexture textures, int cp, int movement)
    {
        this.textures = textures;

        SetTokenArt(textures.Art); 
        
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

    public void SetTokenArt(Texture2D texture)
    {
        Material tokenMaterial = mr.GetComponent<Renderer>().material;
        tokenMaterial.SetTexture("_TokenArt", texture);
    }
}
