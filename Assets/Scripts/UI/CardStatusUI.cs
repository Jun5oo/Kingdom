using TMPro;
using UnityEngine;

/// <summary>
/// 카드의 능력치 UI 클래스 
/// </summary>

public class CardStatusUI : MonoBehaviour, IPoolable
{
    [SerializeField] TextMeshProUGUI cp;
    [SerializeField] TextMeshProUGUI movement;

    public void OnUpdate(int cp, int movement)
    {
        this.cp.text = cp.ToString();
        this.movement.text = movement.ToString();
    }
}
