using TMPro;
using UnityEngine;

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
