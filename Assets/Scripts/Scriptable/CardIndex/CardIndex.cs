using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "CardIndex", menuName = "CardIndex")]
public class CardIndex : ScriptableObject
{
    public class Entry
    {
        public string cardID;
        // 참조할 asset의 타입을 명시하기 위한 AssetReferenceT 
        public AssetReferenceT<CardData> dataReference; 
    }

    [SerializeField] public List<Entry> entries; 
}
