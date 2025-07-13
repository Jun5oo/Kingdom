using System.Collections;
using TMPro;
using UnityEngine;


public class DamagePopupUI : MonoBehaviour, IPoolable
{
    [SerializeField] TextMeshProUGUI damage;

    public void Init(int damage)
    {
        this.damage.text = damage.ToString();
    }
}
