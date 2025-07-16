using TMPro;
using UnityEngine;

public class TokenView : EntityView
{
    [SerializeField] MeshRenderer mr;
    [SerializeField] TextMeshPro cp;
    [SerializeField] TextMeshPro movement;
    [SerializeField] Transform anchorUIPosition;

    public override Transform AnchorUI {  get { return anchorUIPosition; } }

    public void Init(Texture2D texture, int cp, int movement)
    {
        SetTokenArt(texture); 
        
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
