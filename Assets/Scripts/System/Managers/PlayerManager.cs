using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 로컬 플레이어와 원격(AI) 플레이어의 정보를 생성하고 제공하는 매니저.
/// PlayerConfig ScriptableObject에서 선택된 종족을 읽어 Player 인스턴스를 초기화한다.
/// Local / Remote 프로퍼티로 각 플레이어에 빠르게 접근할 수 있다.
/// </summary>
public class PlayerManager : MonoBehaviour
{
    int localPlayerID;
    int remotePlayerID;

    [SerializeField] PlayerConfig config; // 인스펙터에서 종족 선택 정보를 담은 ScriptableObject

    Player localPlayer;
    Player remotePlayer;
    Dictionary<int, Player> playerDict; // 플레이어 ID → Player 빠른 조회

    public Player Local {  get { return localPlayer; } }
    public Player Remote { get { return remotePlayer; } }
    public Dictionary<int, Player> PlayerDict { get {  return playerDict; } }

    /// <summary>
    /// config에서 종족을 읽어 로컬(ID=0)과 원격(ID=1) Player를 생성하고 딕셔너리에 등록한다.
    /// </summary>
    public void Init()
    {
        Player[] players = new Player[2];

        players[0] = new Player(0, config.playerSelected, "Local", true);
        players[1] = new Player(1, config.botSelected, "Remote", false);

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
