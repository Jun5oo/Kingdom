using System.Collections;
using TMPro;
using UnityEngine;


public class DamagePopup : MonoBehaviour, IPoolable
{
    [SerializeField] TextMeshProUGUI damage;

    public void Init(int damage)
    {
        this.damage.text = damage.ToString();
    }
}
