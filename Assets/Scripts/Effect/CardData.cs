using System.Collections.Generic;
using UnityEngine;

public class CardData : ScriptableObject
{
    [SerializeField] string id;
    public string ID { get { return id; } internal set { id = value; } }

    [SerializeField] string cardName; 
    public string Name { get { return cardName; } internal set { cardName = value; } }

    [TextArea] string description; 
    public string Description { get { return description; } internal set { description = value; } }

    [SerializeField] Race race; 
    public Race Race { get { return race; } internal set { race = value; } }

    [SerializeField] UnitTag tag; 
    public UnitTag Tag { get { return tag; } internal set { tag = value; } }

    [SerializeField] int maxLevel;
    public int MaxLevel { get { return maxLevel; } internal set { maxLevel = value; } }

    [SerializeField]
    List<int> cp = new List<int>();
    public List<int> CP { get { return cp; } internal set { cp = value; } }
    
    [SerializeField]
    List<RangeType> attackType = new List<RangeType>();
    public List<RangeType> AttackType { get { return attackType; } internal set { attackType = value; } }

    [SerializeField]
    List<int> attackRange = new List<int>();
    public List<int> AttackRange { get { return attackRange; } internal set { attackRange = value; } }

    [SerializeField]
    List<RangeType> moveType = new List<RangeType>();
    public List<RangeType> MoveType { get { return moveType; } internal set { moveType = value; } }

    [SerializeField]
    List<int> moveRange = new List<int>();
    public List<int> MoveRange { get { return moveRange; } internal set { moveRange = value; } }

    [SerializeField]
    List<EffectData> effects = new List<EffectData>();
    public List<EffectData> Effects { get { return effects; } internal set { effects = value; } }
}
