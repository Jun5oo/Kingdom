using DG.Tweening;
using UnityEngine;

public class Summon_Icon : MonoBehaviour, IHoverable
{
    GridSystem grid;
    CardMovement cardMovement; 

    Vector3 originalScale = Vector3.one;
    Vector3 hoverScale = Vector3.one * 1.1f; 
    
    public void OffHover()
    {
        transform.localScale = originalScale; 
    }

    public void OnHover()
    {
        transform.localScale = hoverScale;
    }

    void OnMouseDown()
    {
        cardMovement = this.transform.parent.GetComponent<CardMovement>();
        grid = GameObject.Find("GridSystem").GetComponent<GridSystem>();
        Vector3 position = grid.GetSingleTestGrid();

        Sequence sequence = DOTween.Sequence();

        sequence.Append(this.transform.parent.DOMove(new Vector3(position.x, this.transform.parent.position.y - 2f, position.z), 0.5f));
        sequence.Join(this.transform.parent.DORotate(new Vector3(0f, 0f, -180f), 0.5f));
        sequence.Append(this.transform.parent.DOMove(position, 0.3f).SetEase(Ease.InBack, 0.2f)
            .OnComplete(() => {
                transform.DOLocalMoveY(this.transform.parent.position.y, 0.15f)
            .SetEase(Ease.InQuad, 5f);
                this.gameObject.SetActive(false);
            }));
        // cardMovement.MoveTransform(new PRS(position, this.transform.parent.rotation, Vector3.one), 0.5f); 

    }

}
