using System;
using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyInfoButton : MonoBehaviour, IPoolable<LobbyInfoButton>
{
    [SerializeField] private Button joinLobbyButton;
    [SerializeField] private TextMeshProUGUI roomNameText;
    [SerializeField] private TextMeshProUGUI playerNumText;

    public event Action<LobbyInfoButton> returnAction;

    private string lobbyCode;

    public void ReturnToPool()
    {
        returnAction?.Invoke(this);
    }

    public void AddJoinEvent(Lobby lobby)
    {
        UpdateLobbyInfo(lobby);

        joinLobbyButton.onClick.AddListener(async () =>
        {
            joinLobbyButton.interactable = false;
            await JoinLobby();
            joinLobbyButton.interactable = true;
        });
    }

    private async Task JoinLobby()
    {
        await MultiplayManager.Instance.LobbyController.JoinLobby(lobbyCode);
    }

    public void UpdateLobbyInfo(Lobby lobby)
    {
        UpdateTexts(lobby.Name, lobby.Players.Count, lobby.MaxPlayers);
        if (lobby.Data.TryGetValue(LobbyDataType.LobbyCode.ToString(), out DataObject data))
        {
            lobbyCode = data.Value;
        }
        else
        {
            Debug.LogError("아직 Data가 로비에 없습니다.");
        }
    }

    private void UpdateTexts(string lobbyName, int PlayerCount, int maxPlayerCount)
    {
        roomNameText.text = lobbyName;
        playerNumText.text = $"{PlayerCount} / {maxPlayerCount}";
    }
}
