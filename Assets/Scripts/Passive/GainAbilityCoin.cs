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

        var abilityResourceSystem = ServiceLocator.Get<IResourceSystem>();
        abilityResourceSystem.Add(attacker.OwnerID, 1); 

        Debug.Log($"PlayerID {attacker.OwnerID} get ability coin!"); 
    }
}
