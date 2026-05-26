using TMPro;
using UnityEngine;

/// <summary>
/// 토큰의 시각 표현을 담당한다.
/// 머티리얼 텍스처(아트/프레임/마스크) 설정과 StatusPresenter를 통한 CP/이동력 표시를 관리한다.
/// </summary>
public class TokenView : BaseView
{
    [SerializeField] MeshRenderer mr;
    [SerializeField] Transform anchorPosition; // 팝업 표시 기준점
    [SerializeField] Canvas statusCanvas;

    StatusPresenter presenter;

    public override Transform Anchor { get { return anchorPosition; } }
    public Canvas Canvas { get { return statusCanvas; } }

    /// <summary> 텍스처를 적용하고 StatusPresenter를 초기화하여 초기 CP/이동력을 표시한다. </summary>
    public void Init(VisualTexture textures, StatusPresenter presenter, int cp, int movement = 1)
    {
        SetTokenView(textures.Art, textures.Frame, textures.Mask);

        this.presenter = presenter;

        presenter.Init();
        presenter.SetStatus(cp, movement);
    }

    public void OnUpdateCP(int cp) => presenter.OnUpdateCP(cp);
    public void OnUpdateMovement(int movement) => presenter.OnUpdateCP(movement);

    /// <summary> 토큰 머티리얼의 아트·프레임·마스크 텍스처를 교체한다. </summary>
    public void SetTokenView(Texture2D art, Texture2D frame, Texture2D artMask)
    {
        Material tokenMaterial = mr.GetComponent<Renderer>().material;

        tokenMaterial.SetTexture("_TokenArt", art);
        tokenMaterial.SetTexture("_TokenFrame", frame);
        tokenMaterial.SetTexture("_TokenFrameMask", artMask);
    }
}
