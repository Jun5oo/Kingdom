using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    PlayerManager playerManager;

    // 임시적으로 정해진 고정덱 
    [SerializeField] List<CardData> undeadDeck;
    [SerializeField] List<CardData> celestialDeck;

    [SerializeField] CardData undead;
    [SerializeField] CardData celestial;

    private Dictionary<int, Queue<CardData>> playerDeck;
    private Dictionary<int, CardData> playerKing; 

    public void Init()
    {
        playerManager = ServiceLocator.Get<PlayerManager>();

        playerDeck = new Dictionary<int, Queue<CardData>>(); 
        playerKing = new Dictionary<int, CardData>();   

        playerDeck[playerManager.Local.PlayerID] = CreateDeck(playerManager.Local.Race);
        playerDeck[playerManager.Remote.PlayerID] = CreateDeck(playerManager.Remote.Race);

        playerKing[playerManager.Local.PlayerID] = CreateKing(playerManager.Local.Race);
        playerKing[playerManager.Remote.PlayerID] = CreateKing(playerManager.Remote.Race); 
    }

    public Queue<CardData> CreateDeck(Race race)
    {
        switch(race){
            case Race.Undead:
                return new Queue<CardData>(undeadDeck);
            case Race.Celestial:
                return new Queue<CardData>(celestialDeck);
        }

        Debug.LogError("Undefined race");
        return null; 
    }
    public CardData CreateKing(Race race)
    {
        switch (race)
        {
            case Race.Undead:
                return undead; 
            case Race.Celestial:
                return celestial; 
        }

        Debug.LogError("Undefined race");
        return null;

    }
    public CardData GetCardData(int playerID)
    {
        if(IsDeckEmpty(playerID)) 
            return null;

        CardData cardData = playerDeck[playerID].Dequeue(); 
        return cardData;
    }
    public CardData GetKingCardData(int playerID)
    {
        return playerKing[playerID]; 
    }
    public bool IsDeckEmpty(int playerID)
    {
        if (!playerDeck.ContainsKey(playerID))
        {
            Debug.LogError("잘못된 플레이어ID 입니다.");
            return false; 
        }

        return playerDeck[playerID].Count <= 0; 
    }
}
