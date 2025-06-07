using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class MultiplayManager : MonoBehaviour
{
    public static MultiplayManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("@MultiplayManager");
                MultiplayManager multiplayManager = go.AddComponent<MultiplayManager>();
                instance = multiplayManager;
            }

            return instance;
        }
    }

    private static MultiplayManager instance;
    private Authentication authentication;
    public LobbyController LobbyController { get ; private set; }

    [SerializeField] private UnityTransport unityTransport;

    private async void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        LobbyController = new LobbyController(unityTransport);
        authentication = new Authentication();
        await authentication.Initialize();

        LobbyController.InvokeRepeatUpdateLobby();
    }
}
