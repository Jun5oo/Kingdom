using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Inspector에서 직접 등록한 CardData 목록을 Name/Race 키 딕셔너리로 관리하는 임시 클래스.
/// CSV 임포터로 생성된 ScriptableObject를 직렬화 필드에 연결해 사용한다.
/// </summary>
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
            dictionary[data.CardName] = data; 

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
