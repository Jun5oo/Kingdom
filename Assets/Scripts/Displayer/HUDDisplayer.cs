using UnityEngine;

/// <summary>
/// 로컬/원격 플레이어의 HUD(체력 등 왕 정보 표시)를 초기화하고 활성화하는 컴포넌트.
/// KingPlacement 완료 후 SetHUD() → ActivateHUD() 순서로 호출된다.
/// </summary>
public class HUDDisplayer : MonoBehaviour
{
    PlayerManager playerManager;
    TokenManager tokenManager;

    [SerializeField] PlayerHUD localHUD;
    [SerializeField] PlayerHUD remoteHUD;

    public void Init()
    {
        playerManager = ServiceLocator.Get<PlayerManager>();
        tokenManager = ServiceLocator.Get<TokenManager>();
    }

    /// <summary> 각 플레이어의 왕 토큰을 찾아 PlayerHUD를 초기화한다. </summary>
    public void SetHUD()
    {
        if (!tokenManager.TryGetKingTokenFrom(playerManager.Local.PlayerID, out Token localKing))
        {
            Debug.LogError($"{playerManager.Local.PlayerName}의 왕을 찾을 수 없습니다");
            return;
        }

        if (!tokenManager.TryGetKingTokenFrom(playerManager.Remote.PlayerID, out Token remoteKing))
        {
            Debug.LogError($"{playerManager.Remote.PlayerName}의 왕을 찾을 수 없습니다");
            return;
        }

        localHUD.Init(playerManager.Local, localKing);
        remoteHUD.Init(playerManager.Remote, remoteKing);
    }

    /// <summary> 로컬/원격 HUD 오브젝트를 활성화한다. </summary>
    public void ActivateHUD()
    {
        localHUD.gameObject.SetActive(true);
        remoteHUD.gameObject.SetActive(true);
    }
}
