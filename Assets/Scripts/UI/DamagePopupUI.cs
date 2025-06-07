using System.Collections;
using TMPro;
using UnityEngine;


public class DamagePopupUI : MonoBehaviour, IPoolable
{
    [SerializeField] TextMeshProUGUI text; 
    public void SetupDamage(int damage)
    {
        text.text = damage.ToString();
    }

    void OnEnable()
    {
        StartCoroutine(HidePopupCoroutine()); 
    }

    IEnumerator HidePopupCoroutine()
    {
        yield return new WaitForSeconds(2f); 
        
        UISystem uiSystem = GameObject.FindAnyObjectByType<UISystem>();
        uiSystem.Push<DamagePopupUI>(this.gameObject); 
    }
}
