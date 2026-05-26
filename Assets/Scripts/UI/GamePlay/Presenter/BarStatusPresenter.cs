using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// King 토큰 전용 HP 슬라이더 프레젠터. SetStatus()로 maxHp를 기록하고 OnUpdateCP()에서 슬라이더 비율을 갱신한다.
/// </summary>
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
