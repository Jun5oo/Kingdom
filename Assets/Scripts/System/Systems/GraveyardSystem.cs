using System.Collections.Generic;

public class GraveyardSystem
{
    Dictionary<int, List<CardData>> graveyardDictionary; 

    public GraveyardSystem()
    {
        graveyardDictionary = new Dictionary<int, List<CardData>>(); 
    }

    public void AddToGraveyard(int playerID, CardData cardData)
    {
        if (!graveyardDictionary.ContainsKey(playerID))
            graveyardDictionary[playerID] = new List<CardData>();

        graveyardDictionary[playerID].Add(cardData); 
    }

    public List<CardData> GetPlayerGraveyard(int playerID)
    {
        if (!graveyardDictionary.ContainsKey(playerID))
            return null;
        
        return graveyardDictionary[playerID];
    }
}
