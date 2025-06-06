using UnityEngine;
using TMPro;

// 턴 관련 UI를 갱신하는 매니저 클래스
public class TurnUIManager : MonoBehaviour
{
    public static TurnUIManager Instance;

    [SerializeField] private TMP_Text turnOwnerText;
    [SerializeField] private TMP_Text actionPointText;

    void Awake()
    {
        Instance = this;
    }

    // 턴 소유자 텍스트 업데이트 (내 턴/상대 턴)
    public void UpdateTurnOwner(bool isMyTurn)
    {
        turnOwnerText.text = isMyTurn ? "나의 턴" : "상대의 턴";
    }

    // 현재 남은 행동력 텍스트 업데이트
    public void UpdateActionPoint(int actionPoint)
    {
        actionPointText.text = $"남은 행동력: {actionPoint}";
    }
}
