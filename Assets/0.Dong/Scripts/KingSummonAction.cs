using UnityEngine;

// 왕 소환(배치) 전용 액션 클래스
public class KingSummonAction : IAction
{
    private IGridSystem gridSystem;
    private IActionSystem actionSystem;
    private Card card; 

    private ActionType actionType = ActionType.KingSummon;
    public ActionType ActionType => actionType;

    // 생성자: 필요한 시스템 및 대상 카드를 주입
    public KingSummonAction(IGridSystem gridSystem, IActionSystem actionSystem, Card card)
    {
        this.gridSystem = gridSystem;
        this.actionSystem = actionSystem;
        this.card = card; 
    }

    // 액션 진입 시 호출됨: 유효 위치 하이라이팅 + 셀 클릭 대기
    public void Enter()
    {
        Exit();

        gridSystem.HighlightGridCells((Vector2Int pos) =>
        {
            return IsValidKingPosition(pos) && !gridSystem.IsObjectOnGridPosition(pos);
        }, HighlightType.ValidSummon, HighlightLayer.Outline);

        gridSystem.OnActionOccured += PlaceKing;
    }

    // 액션 종료 시 호출: 셀 하이라이트 해제 및 이벤트 제거
    public void Exit()
    {
        gridSystem.UnhighlightGridCells(HighlightLayer.Outline);
        gridSystem.OnActionOccured -= PlaceKing;
    }


    // 이 액션이 현재 상황에서 유효한지 여부 (항상 왕은 가능)
    public bool IsValid()
    {
        Card card = this.card; 

        // 왕이라면 카드 상태에 관계없이 항상 소환 가능
        if (card.IsKing)
            return true;

        // 일반 카드는 Hand 상태여야만 유효
        return card.CardState == CardState.Hand;
    }

    // 왕을 실제로 배치하는 함수 (셀 클릭 시 호출됨)
    private void PlaceKing(Vector2Int pos)
    {
        Exit();

        //Temp 
        card.GetComponent<CardView>().HideStatusUI(); 
        UISystem uiSystem = GameObject.FindAnyObjectByType<UISystem>();
        // 

        gridSystem.PlaceObjectTo(card.gameObject, pos);

        Vector3 worldPos = gridSystem.GetWorldPosition(pos) + (Vector3.up * 0.2f);
        Vector3 eulerAngles = card.IsMyCard ? new Vector3(90f, 0f, 0f) : new Vector3(90f, 0f, 180f);
        Quaternion rot = Quaternion.Euler(eulerAngles);
        Vector3 scale = Vector3.one;

        card.gameObject.SetActive(true); 
        var cardMove = card.gameObject.GetComponent<CardMovement>();
        cardMove.MoveTransform(new PRS(worldPos, rot, scale), 0.5f, false, () =>
        {
            card.CardState = CardState.Field;
            actionSystem?.CancelAction();
            // Temp 
            GameObject.FindAnyObjectByType<GameFlowManager>()?.OnKingPlaced();
            //
        });
    }

    // 왕이 배치 가능한 셀인지 여부 판별
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
