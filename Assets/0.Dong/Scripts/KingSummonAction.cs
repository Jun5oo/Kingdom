using UnityEngine;

public class KingSummonAction : IAction
{
    private IGridSystem gridSystem;
    private IActionSystem actionSystem;
    private Card card; 

    private ActionType actionType = ActionType.KingSummon;
    public ActionType ActionType => actionType;

    public KingSummonAction(IGridSystem gridSystem, IActionSystem actionSystem, Card card)
    {
        this.gridSystem = gridSystem;
        this.actionSystem = actionSystem;
        this.card = card; 
    }

    public void Enter()
    {
        Exit();

        gridSystem.HighlightGridCells((Vector2Int pos) =>
        {
            return IsValidKingPosition(pos) && !gridSystem.IsObjectOnGridPosition(pos);
        });

        gridSystem.OnActionOccured += PlaceKing;
    }

    public void Exit()
    {
        gridSystem.UnhighlightGridCells();
        gridSystem.OnActionOccured -= PlaceKing;
    }

    public bool IsValid()
    {
        Card card = this.card; 

        // 왕이라면 카드 상태에 관계없이 항상 소환 가능
        if (card.IsKing)
            return true;

        // 일반 카드는 Hand 상태여야만 유효
        return card.CardState == CardState.Hand;
    }

    private void PlaceKing(Vector2Int pos)
    {
        Exit();

        //Temp 
        card.GetComponent<CardView>().HideStatusUI(); 
        UISystem uiSystem = GameObject.FindAnyObjectByType<UISystem>();
        // 

        gridSystem.PlaceObjectTo(card.gameObject, pos);

        Vector3 worldPos = gridSystem.GetWorldPosition(pos);
        Quaternion rot = Quaternion.Euler(0, 0, -180);
        Vector3 scale = Vector3.one;

        card.gameObject.SetActive(true); 
        var cardMove = card.gameObject.GetComponent<CardMovement>();
        cardMove.MoveTransform(new PRS(worldPos, rot, scale), 0.5f, false, () =>
        {
            actionSystem?.CancelAction();
            // Temp 
            card.GetComponent<CardView>().DisplayStatusUI(); 
            //
        });
    }

    private bool IsValidKingPosition(Vector2Int pos)
    {
        Card card = this.card; 
        if (card.IsMyCard)
        {   
            // 내 왕은 아래쪽 (y < 3)
            return pos.y < 3;
        }
        else
        {
            // 적 왕은 위쪽 (y > 4)
            return pos.y > 4;
        }
    }
}
