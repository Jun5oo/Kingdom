using UnityEngine;

public class GainAbilityCoin : IPassive
{
    // 아군 유닛이 적을 처치했을 때 발동되는 패시브 
    BaseObject baseObject; 
    DamageManager damageManager; 

    public GainAbilityCoin(BaseObject baseObject)
    {
        damageManager = ServiceLocator.Get<DamageManager>();    
        this.baseObject = baseObject;
    }

    public void Activate()
    {
        Debug.Log($"{this} event connected"); 
        damageManager.OnPlayerUnitKilledEnemy += GetAbilityCoin;
    }
    public void Deactivate() => damageManager.OnPlayerUnitKilledEnemy -= GetAbilityCoin; 
    public void GetAbilityCoin(Token attacker, Token defender)
    {
        if (baseObject.OwnerID != attacker.OwnerID)
            return;

        // 현재는 처치한 Token의 정보를 사용하지는 않지만, 추후 로그 등 기록을 남길 때 사용할 수 있으므로 남겨둠. 

        Debug.Log($"PlayerID {attacker.OwnerID} get ability coin!"); 
    }
}
