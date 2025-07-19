public class Player
{
    private int playerID;
    private string playerName;
    private Race race;
    private bool isLocal;

    public Token kingToken; 

    public int PlayerID { get { return playerID; } }
    public Race Race { get { return race; } }
    public string PlayerName { get { return playerName; } } 
    public bool IsLocal {  get { return isLocal; } }

    public Player(int playerID, Race race, string playerName, bool isLocal)
    {
        this.playerID = playerID;
        this.race = race;
        this.playerName = playerName;
        this.isLocal = isLocal;
    }
}
