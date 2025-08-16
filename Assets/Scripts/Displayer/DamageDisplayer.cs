using UnityEngine;

public class DamageDisplayer 
{
    // 데미지 팝업 Displayer. 데미지 텍스트를 풀링, Play
    PoolManager poolManager;

    public DamageDisplayer()
    {
        poolManager = ServiceLocator.Get<PoolManager>();
    }

    public void Display(int damage, BaseObject baseObject)
    {
        DamagePopup damagePopup = poolManager.Pop<DamagePopup>();

        damagePopup.transform.position = Camera.main.WorldToScreenPoint(baseObject.transform.position); 
        damagePopup.Init(damage);
        damagePopup.Play(()=> poolManager.Push<DamagePopup>(damagePopup)); 
    }
}
