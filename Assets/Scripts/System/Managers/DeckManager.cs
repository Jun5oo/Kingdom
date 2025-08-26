using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    PlayerManager playerManager;

    [SerializeField] DeckSO deck;
    [SerializeField] CommanderSO commander; 

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
                return new Queue<CardData>(deck.undeadDeck);
            case Race.Celestial:
                return new Queue<CardData>(deck.celestialDeck);
        }

        Debug.LogError("Undefined race");
        return null; 
    }
    public CardData CreateKing(Race race)
    {
        switch (race)
        {
            case Race.Undead:
                return commander.undead; 
            case Race.Celestial:
                return commander.celestial;
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
