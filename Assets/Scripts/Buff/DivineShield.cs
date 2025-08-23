using Cysharp.Threading.Tasks;
using UnityEngine;

public class DivineShield : IBuff, IDamageModifierBuff
{
    TurnSystem turnSystem;
    
    IBuffable target;

    GameObject shield = null; 

    public IBuffable Target { get { return target; } }
    
    int duration = 2;

    public DivineShield(IBuffable target)
    {
        this.target = target;

        this.turnSystem = ServiceLocator.Get<TurnSystem>();

        turnSystem.onTurnStarted -= ReduceHandler;
        turnSystem.onTurnStarted += ReduceHandler; 
    }

    public async UniTask OnApply()
    {
        Debug.Log("DivineShield Applied");
        PrefabLoader prefabLoader = ServiceLocator.Get<PrefabLoader>();
        GameObject prefab = await prefabLoader.LoadPrefabAsync("divineShieldPrefab"); 

        if(target is MonoBehaviour monoBehaviour)
        {
            shield = GameObject.Instantiate(prefab, monoBehaviour.transform);
            shield.name = "DivineShield"; 
        }

        target.AddBuff(this); 
    }

    public void OnRemove()
    {
        Debug.Log("DivineShield Removed");
        
        shield.SetActive(false);

        if (shield != null)
            GameObject.Destroy(shield);

        if (target is BaseObject baseObject)
            turnSystem.onTurnStarted -= ReduceHandler;

        if (target != null)
            target.RemoveBuff(this); 
    }

    public void ReduceDuration()
    {
        duration -= 1; 

        if (IsExpired())
            OnRemove(); 
    }

    public void ReduceHandler(int playerID)
    {
        int ownerID = -1;

        if (target is BaseObject baseObject)
            ownerID = baseObject.OwnerID;
        else
            Debug.Log($"{target} is not baseObject"); 

        if (playerID == ownerID)
            ReduceDuration(); 
    }

    public bool IsExpired()
    {
        return duration <= 0; 
    }

    public int ModifyDamage(int damage)
    {
        Debug.Log("Modified"); 
        damage = Mathf.FloorToInt(damage / 2);

        if(Target is Token token)
        {
            if (token.CP - damage <= 0)
                damage = token.CP - 1;
        }

        duration = 0; 
        OnRemove();
        return damage; 
    }

    public bool IsStackable() => false; 
}
