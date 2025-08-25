using TMPro;
using UnityEngine;

public class NumberStatusPresenter : StatusPresenter
{
    [SerializeField] TextMeshProUGUI cp;
    [SerializeField] TextMeshProUGUI movement; 
    public override void Init()
    {
        if(cp == null)
        {
            Debug.LogError("CP Text를 찾을 수 없습니다");
            return; 
        }

        if(movement == null)
        {
            Debug.LogError("Movement Text를 찾을 수 없습니다.");
            return; 
        }
    }

    public override void SetStatus(int cp, int movement = 1)
    {
        this.cp.text = cp.ToString(); 
        this.movement.text = movement.ToString();
    }

    public override void OnUpdateCP(int cp)
    {
        this.cp.text = cp.ToString(); 
    }
}
