using TMPro;
using UnityEngine;

public class TokenView : EntityView
{
    [SerializeField] SpriteRenderer sr; 
    [SerializeField] TextMeshPro cp;
    [SerializeField] TextMeshPro movement;
    [SerializeField] Transform anchorUIPosition;

    public override Transform AnchorUI {  get { return anchorUIPosition; } }

    public void Init(Sprite sprite, int cp, int movement)
    {
        this.sr.sprite = sprite;
        this.cp.text = cp.ToString();
        this.movement.text = movement.ToString();
    }

    public void OnUpdateCP(int cp)
    {
        this.cp.text = cp.ToString(); 
    }
}
