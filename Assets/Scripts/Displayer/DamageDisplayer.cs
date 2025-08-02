using UnityEngine;

public class DamageDisplayer 
{
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
