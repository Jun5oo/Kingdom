using System.Collections.Generic;
using UnityEngine;

public class CardDataSO : ScriptableObject
{
    public string ID;
    public string Name;
    [TextArea] public string Description;

    public Race Race;
    public UnitTag Tag;

    public int MaxLevel; 
    
    public List<int> CP = new List<int>();
    public List<RangeType> AttackType = new List<RangeType>();
    public List<int> AttackRange = new List<int>();  
    public List<RangeType> MoveType = new List<RangeType>();
    public List<int> MoveRange = new List<int>();

    public List<EffectData> Effects = new List<EffectData>();  
}
