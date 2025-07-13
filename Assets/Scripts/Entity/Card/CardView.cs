using UnityEngine;

public class CardView : EntityView
{
    [SerializeField] SpriteRenderer sr;
    [SerializeField] Transform anchorUIPosition; 
    public override Transform AnchorUI { get { return anchorUIPosition; } }

    public void Init(Sprite sprite)
    {
        sr.sprite = sprite;
    }
}
