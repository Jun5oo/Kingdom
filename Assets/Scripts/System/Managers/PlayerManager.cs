using System;
using System.Collections.Generic;

public class PlayerManager
{
    int localPlayerID;
    int remotePlayerID;

    PlayerData localPlayerData;
    PlayerData remotePlayerData;
    Dictionary<int, PlayerData> playerDict;

    public PlayerData LocalPlayerData {  get { return localPlayerData; } }
    public PlayerData RemotePlayerData { get { return remotePlayerData; } }
    public Dictionary<int, PlayerData> PlayerDict { get {  return playerDict; } }

    public void Init(PlayerData[] playerData)
    {
        if(playerData.Length != 2)
            throw new ArgumentException("Invalid PlayerData");

        foreach (var player in playerData)
        {
            if (player.IsLocal)
            {
                this.localPlayerData = player;
                this.localPlayerID = player.PlayerID; 
            }

            else
            {
                this.remotePlayerData = player;
                this.remotePlayerID = player.PlayerID;
            }
        }

        playerDict = new Dictionary<int, PlayerData>();

        playerDict[localPlayerID] = localPlayerData;
        playerDict[remotePlayerID] = remotePlayerData;
    }
}
