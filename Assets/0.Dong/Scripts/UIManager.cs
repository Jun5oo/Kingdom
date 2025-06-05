using System.Collections;
using UnityEngine;

using TMPro;


public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [SerializeField] private TMP_Text turnOrderText;

    void Awake()
    {
        Instance = this;
    }

    public void ShowTurnOrder(string message)
    {
        turnOrderText.text = message;
        turnOrderText.gameObject.SetActive(true);

        StartCoroutine(HideTurnOrder());
    }

    private IEnumerator HideTurnOrder()
    {
        yield return new WaitForSeconds(2f);
        turnOrderText.gameObject.SetActive(false);
    }
}
