using UnityEngine;
using TMPro;

public class TurnUIManager : MonoBehaviour
{
    public static TurnUIManager Instance;

    [SerializeField] private TMP_Text turnOwnerText;
    [SerializeField] private TMP_Text actionPointText;

    void Awake()
    {
        Instance = this;
    }

    public void UpdateTurnOwner(bool isMyTurn)
    {
        turnOwnerText.text = isMyTurn ? "나의 턴" : "상대의 턴";
    }

    public void UpdateActionPoint(int actionPoint)
    {
        actionPointText.text = $"남은 행동력: {actionPoint}";
    }
}
