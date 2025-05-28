using UnityEngine;

public class GameStarter : MonoBehaviour
{
    [SerializeField] private GameObject kingCardPrefab;

    [SerializeField] private IUISystem uiSystem;
    [SerializeField] private IGridSystem gridSystem;
    [SerializeField] private IActionSystem actionSystem;

    private void Start()
    {
        // 왕 카드 생성 (임시 위치: 화면 밖 또는 손 위치)
        Vector3 spawnPos = GameObject.Find("Hand").transform.position;
        GameObject kingCard = Instantiate(kingCardPrefab, spawnPos, Quaternion.identity);

        // Card 컴포넌트 초기화
        Card card = kingCard.GetComponent<Card>();
        card.Init(uiSystem, gridSystem, actionSystem, true); // isMyCard = true

        // 액션 시스템에 왕 배치 액션 진입
        KingSummonAction kingAction = new KingSummonAction(gridSystem, actionSystem, kingCard);
        actionSystem.EnterAction(kingAction);
    }
}
