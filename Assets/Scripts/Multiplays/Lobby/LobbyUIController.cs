using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUIController : MonoBehaviour
{
    [SerializeField] private Button createLobbyButton;
    [SerializeField] private Button enterLobbyButton;

    [SerializeField] private LobbyInfoButton lobbyInfoButtonPrefab;
    [SerializeField] private Transform contentTransform;
    [SerializeField] private int initCount = 10;
    [SerializeField] private int countMax = 100;
    private ObjectPooler<LobbyInfoButton> lobbyInfoPooler;

    private void Start()
    {
        lobbyInfoPooler = new ObjectPooler<LobbyInfoButton>(lobbyInfoButtonPrefab, contentTransform, initCount, countMax);

        MultiplayManager.Instance.LobbyController.OnCreateRoom += CreateRoom;
        MultiplayManager.Instance.LobbyController.OnUpdateLobby += UpdateLobbyUI;

        createLobbyButton.onClick.AddListener(async () => 
        {
            createLobbyButton.interactable = false;
            await MultiplayManager.Instance.LobbyController.CreateLobby();
            createLobbyButton.interactable = true;
        });

        enterLobbyButton.onClick.AddListener(async () =>
        {
            enterLobbyButton.interactable = false;
            await MultiplayManager.Instance.LobbyController.JoinLobby();
            enterLobbyButton.interactable = true;
        });

    }

    // TODO: 갱신할 때 삭제도 필요 룸이 바뀌었는지도 갱신
    private void UpdateLobbyUI(List<Lobby> lobbies)
    {
        int diff = lobbies.Count - lobbyInfoPooler.GetActivePoolCount();

        List<LobbyInfoButton> lobbyInfoButtons = lobbyInfoPooler.GetActiveObjects();

        if (diff > 0)
        {
            for (int i = 0; i < diff; i++)
            {
                CreateRoom(lobbies[i]);
            }
        }
        else if (diff < 0)
        {
            for (int i = 0; i < -diff; i++)
            {
                lobbyInfoButtons[i].ReturnToPool();
            }
        }


        for (int i = 0; i < lobbies.Count; i++)
        {
            lobbyInfoButtons[i].UpdateLobbyInfo(lobbies[i]);
        }
    }

    private void CreateRoom(Lobby lobby)
    {
        LobbyInfoButton lobbyInfoButton = lobbyInfoPooler.Pool();
        lobbyInfoButton.AddJoinEvent(lobby);
    }

    private void DestroyRoom(Lobby lobby)
    {
        
    }
}
