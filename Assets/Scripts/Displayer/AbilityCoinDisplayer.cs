using TMPro;
using UnityEngine;

/// <summary>
/// 어빌리티 코인 현재 수를 텍스트로 표시하는 컴포넌트.
/// IsLocal 플래그로 로컬/원격 플레이어를 구분하여 매 프레임 갱신한다.
/// </summary>
public class AbilityCoinDisplayer : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;

    AbilityResourceSystem abilitySystem;

    [SerializeField] bool IsLocal; // true: 로컬 플레이어, false: 원격 플레이어

    int playerID = -1;

    void Start()
    {
        PlayerManager playerManager = ServiceLocator.Get<PlayerManager>();
        abilitySystem = ServiceLocator.Get<AbilityResourceSystem>();

        if (IsLocal)
            playerID = playerManager.Local.PlayerID;
        else
            playerID = playerManager.Remote.PlayerID;
    }

    private void Update()
    {
        if (abilitySystem != null)
            text.text = abilitySystem.GetCurrentResources(playerID).ToString();
    }
}
