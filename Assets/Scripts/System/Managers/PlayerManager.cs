using System.Collections.Generic;

public class PlayerManager
{
    int localPlayerID;
    int remotePlayerID;

    Player localPlayer;
    Player remotePlayer;
    Dictionary<int, Player> playerDict;

    public Player Local {  get { return localPlayer; } }
    public Player Remote { get { return remotePlayer; } }
    public Dictionary<int, Player> PlayerDict { get {  return playerDict; } }

    public void Init()
    {
        Player[] players = new Player[2];

        players[0] = new Player(0, Race.Undead, "Local", true);
        players[1] = new Player(1, Race.Celestial, "Remote", false);


        foreach (var player in players)
        {
            if (player.IsLocal)
            {
                this.localPlayer = player;
                this.localPlayerID = player.PlayerID; 
            }

            else
            {
                this.remotePlayer = player;
                this.remotePlayerID = player.PlayerID;
            }
        }

        playerDict = new Dictionary<int, Player>();

        playerDict[localPlayerID] = localPlayer;
        playerDict[remotePlayerID] = remotePlayer;
    }
}
