using Cysharp.Threading.Tasks;
using UnityEngine;

public class DivineShield : IBuff, IDamageModifierBuff
{
    TurnSystem turnSystem;
    PlayerManager playerManager;
    IBuffable target;

    GameObject shield = null; 

    public IBuffable Target { get { return target; } }
    int duration = 2;

    public DivineShield(IBuffable target)
    {
        this.target = target;

        this.turnSystem = ServiceLocator.Get<TurnSystem>();
        this.playerManager = ServiceLocator.Get<PlayerManager>();

        turnSystem.OnPlayerTurnStarted -= ReduceDuration;
        turnSystem.OnOpponentTurnStarted -= ReduceDuration; 

        if(target is BaseObject baseObject)
        {
            if (baseObject.OwnerID == playerManager.Local.PlayerID)
                turnSystem.OnPlayerTurnStarted += ReduceDuration;
            else
                turnSystem.OnOpponentTurnStarted += ReduceDuration; 
        } 
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

        if (target is BaseObject baseObject)
        {
            if (baseObject.OwnerID == playerManager.Local.PlayerID)
                turnSystem.OnPlayerTurnStarted -= ReduceDuration;
            else
                turnSystem.OnOpponentTurnStarted -= ReduceDuration;
        }
    }

    public void ReduceDuration()
    {
        Debug.Log("Reduced"); 
        duration -= 1;

        if (IsExpired())
            OnRemove(); 
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
