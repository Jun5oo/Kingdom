using UnityEngine;

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

    public void ActivateHUD()
    {
        localHUD.gameObject.SetActive(true);
        remoteHUD.gameObject.SetActive(true);
    }
}
