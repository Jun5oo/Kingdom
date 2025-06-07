using NUnit.Framework.Constraints;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using UnityEngine;
using UnityEngine.Events;

public class LobbyController
{
    public event UnityAction<Lobby> OnCreateRoom;
    public event UnityAction<List<Lobby>> OnUpdateLobby;

    [SerializeField] private UnityTransport unityTransport;

    // 임시
    private int roomNum = 1;

    private Lobby currentLobby;

    public LobbyController(UnityTransport unityTransport)
    {
        this.unityTransport = unityTransport;
    }

    private async void OnApplicationQuit()
    {
        await TryDeleteLobbyIfHost();
    }

    private async void OnDestroy()
    {
        await TryDeleteLobbyIfHost();
    }

    private async Task TryDeleteLobbyIfHost()
    {
        if (currentLobby != null && currentLobby.HostId == AuthenticationService.Instance.PlayerId)
        {
            try
            {
                await LobbyService.Instance.DeleteLobbyAsync(currentLobby.Id);
                Debug.Log("종료 시 로비 삭제 성공");
            }
            catch (Exception e)
            {
                Debug.LogWarning($"종료 시 로비 삭제 실패: {e.Message}");
            }
        }
    }

    public void InvokeRepeatUpdateLobby()
    {
        MultiplayManager.Instance.StartCoroutine(CoRefreshRecursion());
    }

    public IEnumerator CoRefreshRecursion()
    {
        yield return new WaitForSeconds(3f);
        yield return RefreshLobby();
    }

    private async Task RefreshLobby()
    {
        try
        {
            QueryLobbiesOptions options = new QueryLobbiesOptions
            {
                Count = 25,
                /*Filters = new List<QueryFilter>
                {
                    new QueryFilter(
                        field: QueryFilter.FieldOptions.AvailableSlots,
                        op: QueryFilter.OpOptions.GT,
                        value: "0")
                },*/
                Order = new List<QueryOrder>
                {
                    // 생성된 순서로 정렬
                    new QueryOrder(
                        asc: false,
                        field: QueryOrder.FieldOptions.Created)
                }
            };

            QueryResponse response = await LobbyService.Instance.QueryLobbiesAsync(options);
            OnUpdateLobby?.Invoke(response.Results);
        }
        catch (LobbyServiceException e)
        {
            Debug.LogError($"로비 목록을 가져오는 데 실패했습니다: {e.Message}");
        }

        MultiplayManager.Instance.StartCoroutine(CoRefreshRecursion());
    }

    public async Task CreateLobby()
    {
        int maxConnection = 2;
        // Relay 생성
        var allocation = await RelayService.Instance.CreateAllocationAsync(maxConnection);

        // relay joincode
        string relayCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

        try
        {
            var createOptions = new CreateLobbyOptions
            {
                IsPrivate = false,
                Data = new Dictionary<string, DataObject>
                {
                    { LobbyDataType.RelayCode.ToString(), new DataObject(DataObject.VisibilityOptions.Public, relayCode) },
                }
            };

            // 로비 생성
            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync($"Room{roomNum++}", 2, createOptions);

            // 로비 정보 업데이트
            lobby = await LobbyService.Instance.UpdateLobbyAsync(lobby.Id, new UpdateLobbyOptions
            {
                IsPrivate = false,
                Data = new Dictionary<string, DataObject>
                {
                    { LobbyDataType.RelayCode.ToString(), new DataObject(DataObject.VisibilityOptions.Public, relayCode) },
                    { LobbyDataType.LobbyCode.ToString(), new DataObject(DataObject.VisibilityOptions.Public, lobby.LobbyCode) }
                }
            });

            // ngo + relay 연결
            var relayServerData = new RelayServerData(
                    allocation.RelayServer.IpV4,
                    (ushort)allocation.RelayServer.Port,
                    allocation.AllocationIdBytes,
                    allocation.ConnectionData,
                    allocation.ConnectionData, // 호스트면 동일한 데이터
                    allocation.Key,
                    false // UDP 사용 시 false
                );

            unityTransport.SetRelayServerData(relayServerData);

            // 호스트 시작
            NetworkManager.Singleton.StartHost();
            currentLobby = lobby;
            OnCreateRoom?.Invoke(lobby);
        }
        catch (Exception e)
        {
            Debug.LogError($"Error in CreateLobby {e}");
        }
    }


    public async Task JoinLobby()
    {
        try
        {
            currentLobby = await LobbyService.Instance.QuickJoinLobbyAsync();
            Debug.Log("로비에 입장했습니다");
        }
        catch (Exception e)
        {
            Debug.LogError($"Error in JoinLobby {e}");
        }
    }

    public async Task JoinLobby(string lobbyCode)
    {
        try
        {
            currentLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode);
            Debug.Log("코드를 입력하여 로비에 입장했습니다");
        }
        catch (Exception e)
        {
            Debug.LogError($"Error in JoinLobby {e}");
        }
    }

}
