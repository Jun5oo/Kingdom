using UnityEngine;

/// <summary>
/// 카드/토큰 시각 표현에 필요한 텍스처 묶음. Art, Mask, Frame, Background, Back 5개 슬롯으로 구성된다.
/// CardTextureLoader/TokenTextureLoader가 Addressables에서 로드한 결과를 담아 Factory에 전달한다.
/// </summary>
public class VisualTexture
{
    protected Texture2D cardArt;
    protected Texture2D artMask;
    protected Texture2D frame;
    
    protected Texture2D background;
    protected Texture2D cardBack; 

    public VisualTexture(Texture2D cardArt, Texture2D artMask, Texture2D frame, Texture2D background = null, Texture2D cardBack = null)
    {
        this.cardArt = cardArt;
        this.artMask = artMask;
        this.frame = frame;
        
        this.background = background;
        this.cardBack = cardBack; 
    }

    public Texture2D Art {  get { return cardArt; } }
    public Texture2D Mask { get { return artMask; } }
    public Texture2D Frame { get {  return frame; } }
    public Texture2D Background { get {  return background; } }
    public Texture2D Back { get { return cardBack; } }  
}
