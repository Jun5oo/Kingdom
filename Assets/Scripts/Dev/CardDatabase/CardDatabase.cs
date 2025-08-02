using System.Collections.Generic;
using UnityEngine;

public class CardDatabase : MonoBehaviour 
{
    [SerializeField] List<UnitCardData> database;
    Dictionary<string, UnitCardData> dictionary; 

    // 카드 데이터를 가져올 임시적인 클래스 
    public void Init()
    {
        dictionary = new Dictionary<string, UnitCardData>();
        
        foreach(UnitCardData data in database)
            dictionary[data.Name] = data; 
    }

    public UnitCardData GetData(string name)
    {
        if(dictionary.TryGetValue(name, out UnitCardData data))
            return data;

        return null; 
    } 
}
