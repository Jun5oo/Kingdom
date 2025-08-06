using System.Collections.Generic;
using UnityEngine;

public class CardDatabase : MonoBehaviour 
{
    [SerializeField] List<CardData> database;
    // cardID, cardData 
    Dictionary<string, CardData> dictionary; 

    // 카드 데이터를 가져올 임시적인 클래스 
    public void Init()
    {
        dictionary = new Dictionary<string, CardData>();
        
        foreach(CardData data in database)
            dictionary[data.Name] = data; 
    }

    public T GetCardData<T>(string name) where T : CardData
    {
        if(dictionary.TryGetValue(name, out CardData data))
            return data as T;

        return null; 
    } 
}
