/// <summary>
/// 플레이어의 ID, 종족, 이름, 로컬 여부를 보관하는 데이터 클래스.
/// PlayerManager에서 생성하며 서비스 전반에서 읽기 전용으로 사용된다.
/// </summary>
public class Player
{
    private int playerID;
    private string playerName;
    private Race race;
    private bool isLocal;

    public Token kingToken; // 배치된 왕 토큰 참조 (HUD 등 외부 접근용)

    public int PlayerID { get { return playerID; } }
    public Race Race { get { return race; } }
    public string PlayerName { get { return playerName; } }
    public bool IsLocal { get { return isLocal; } }

    public Player(int playerID, Race race, string playerName, bool isLocal)
    {
        this.playerID = playerID;
        this.race = race;
        this.playerName = playerName;
        this.isLocal = isLocal;
    }
}
