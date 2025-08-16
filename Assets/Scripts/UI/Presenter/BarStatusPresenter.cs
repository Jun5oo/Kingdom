using UnityEngine;
using UnityEngine.UI;

public class BarStatusPresenter : StatusPresenter
{
    Slider hpSlider;
    int maxHp; 

    public override void Init()
    {
        hpSlider = GetComponent<Slider>();

        if(hpSlider == null)
        {
            Debug.LogError("HP Slider를 찾을 수 없습니다.");
            return; 
        }

        hpSlider.value = 1; 
    }

    public override void SetStatus(int cp, int movement = 1)
    {
        maxHp = cp; 
    }


    public override void OnUpdateCP(int cp)
    {
        hpSlider.value = (float)cp / maxHp; 
    }


}
