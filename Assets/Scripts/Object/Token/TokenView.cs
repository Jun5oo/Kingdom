using TMPro;
using UnityEngine;

public class TokenView : BaseView
{
    [SerializeField] MeshRenderer mr;
    [SerializeField] Transform anchorPosition;
    [SerializeField] Canvas statusCanvas; 
    
    StatusPresenter presenter; 

    public override Transform Anchor {  get { return anchorPosition; } }
    public Canvas Canvas { get { return statusCanvas; } }

    public void Init(VisualTexture textures, StatusPresenter presenter, int cp, int movement = 1)
    {
        SetTokenView(textures.Art, textures.Frame, textures.Mask);

        this.presenter = presenter; 

        presenter.Init();
        presenter.SetStatus(cp, movement); 
    }

    public void OnUpdateCP(int cp) => presenter.OnUpdateCP(cp);
    public void OnUpdateMovement(int movement) => presenter.OnUpdateCP(movement);

    public void SetTokenView(Texture2D art, Texture2D frame, Texture2D artMask)
    {
        Material tokenMaterial = mr.GetComponent<Renderer>().material;

        tokenMaterial.SetTexture("_TokenArt", art);
        tokenMaterial.SetTexture("_TokenFrame", frame);
        tokenMaterial.SetTexture("_TokenFrameMask", artMask); 
    }

}
