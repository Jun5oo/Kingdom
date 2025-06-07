using System.Collections;
using UnityEngine;

using TMPro;

// 턴 관련 메시지를 UI에 표시하는 매니저 클래스
public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [SerializeField] private TMP_Text turnOrderText;

    void Awake()
    {
        Instance = this;
    }

    // 선공/후공 메시지를 표시하고 2초 뒤 자동 숨김
    public void ShowTurnOrder(string message)
    {
        turnOrderText.text = message;
        turnOrderText.gameObject.SetActive(true);

        StartCoroutine(HideTurnOrder());
    }

    // 2초 후 turnOrderText 비활성화 (자동 숨김용)
    private IEnumerator HideTurnOrder()
    {
        yield return new WaitForSeconds(2f);
        turnOrderText.gameObject.SetActive(false);
    }

    // 왕 배치 등 상황에 따라 수동으로 메시지 표시
    public void ShowTurnMessage(string message)
    {
        turnOrderText.text = message;
        turnOrderText.gameObject.SetActive(true);
    }

    // 수동 메시지 숨김 처리
    public void HideTurnMessage()
    {
        turnOrderText.gameObject.SetActive(false);
    }
}
