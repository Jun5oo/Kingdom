using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 신성 방패 버프. DivineShieldAction으로 적용되며 2턴 지속된다.
/// 데미지를 절반으로 줄이고(최소 1 HP 보장), 첫 데미지 수신 시 즉시 만료된다.
/// 소유자 플레이어 턴 시작마다 duration이 1 감소하고, 0이 되면 자동 제거된다.
/// </summary>
public class DivineShield : IBuff, IDamageModifierBuff
{
    TurnSystem turnSystem;
    PlayerManager playerManager;
    IBuffable target;

    GameObject shield = null; // 시각 효과 오브젝트

    public IBuffable Target { get { return target; } }
    int duration = 2; // 남은 지속 턴 수

    /// <summary>
    /// 생성자. 소유자가 로컬이면 OnPlayerTurnStarted, 아니면 OnOpponentTurnStarted에 구독한다.
    /// 중복 구독을 방지하기 위해 먼저 해제한 뒤 등록한다.
    /// </summary>
    public DivineShield(IBuffable target)
    {
        this.target = target;

        this.turnSystem = ServiceLocator.Get<TurnSystem>();
        this.playerManager = ServiceLocator.Get<PlayerManager>();

        turnSystem.OnPlayerTurnStarted -= ReduceDuration;
        turnSystem.OnOpponentTurnStarted -= ReduceDuration;

        if (target is BaseObject baseObject)
        {
            if (baseObject.OwnerID == playerManager.Local.PlayerID)
                turnSystem.OnPlayerTurnStarted += ReduceDuration;
            else
                turnSystem.OnOpponentTurnStarted += ReduceDuration;
        }
    }

    /// <summary> 방패 시각 효과를 생성하고 target에 버프를 등록한다. </summary>
    public async UniTask OnApply()
    {
        Debug.Log("DivineShield Applied");
        PrefabLoader prefabLoader = ServiceLocator.Get<PrefabLoader>();
        GameObject prefab = await prefabLoader.LoadPrefabAsync("divineShieldPrefab");

        if (target is MonoBehaviour monoBehaviour)
        {
            shield = GameObject.Instantiate(prefab, monoBehaviour.transform);
            shield.name = "DivineShield";
        }

        target.AddBuff(this);
    }

    /// <summary> 시각 효과를 파괴하고 턴 이벤트 구독을 해제한 뒤 버프를 제거한다. </summary>
    public void OnRemove()
    {
        Debug.Log("DivineShield Removed");

        shield.SetActive(false);

        if (shield != null)
            GameObject.Destroy(shield);

        if (target is BaseObject baseObject)
        {
            if (baseObject.OwnerID == playerManager.Local.PlayerID)
                turnSystem.OnPlayerTurnStarted -= ReduceDuration;
            else
                turnSystem.OnOpponentTurnStarted -= ReduceDuration;
        }

        if (target != null)
            target.RemoveBuff(this);
    }

    /// <summary> 소유자 플레이어 턴 시작 시 duration을 1 감소하고 만료이면 OnRemove를 호출한다. </summary>
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

    /// <summary>
    /// 데미지를 절반으로 줄인다. 남은 CP에서 절반 데미지를 빼면 0 이하가 되는 경우 CP-1로 제한(즉사 방지).
    /// 데미지 수신 후 즉시 만료된다.
    /// </summary>
    public int ModifyDamage(int damage)
    {
        Debug.Log("Modified");
        damage = Mathf.FloorToInt(damage / 2);

        if (Target is Token token)
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
