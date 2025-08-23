using System.Collections.Generic;
using UnityEngine;

public class CardDatabase : MonoBehaviour 
{
    [SerializeField] List<CardData> database;

    // cardID, cardData 
    Dictionary<string, CardData> dictionary;
    Dictionary<Race, List<CardData>> raceDictionary; 

    // 카드 데이터를 가져올 임시적인 클래스 
    public void Init()
    {
        dictionary = new Dictionary<string, CardData>();
        
        foreach(CardData data in database)
            dictionary[data.ID] = data; 

        raceDictionary = new Dictionary<Race, List<CardData>>();  
        
        foreach(var data in database)
        {
            if (!raceDictionary.ContainsKey(data.Race))
                raceDictionary[data.Race] = new List<CardData>();

            raceDictionary[data.Race].Add(data); 
        }
    }

    public T GetCardData<T>(string name) where T : CardData
    {
        if(dictionary.TryGetValue(name, out CardData data))
            return data as T;

        return null; 
    }
    
    public List<CardData> GetRaceCardList(Race race)
    {
        if (raceDictionary.TryGetValue(race, out List<CardData> cardList))
            return cardList;

        return null; 
    }
}
