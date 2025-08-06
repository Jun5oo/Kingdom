using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitCardData", menuName = "Card Scriptable/Unit")]
public class UnitCardData : CardData
{
    [SerializeField] int cp;
    [SerializeField] int movement;
    [SerializeField] bool isKing;
    
    [SerializeField] UnitTag tag; 

    [SerializeField] List<Vector2Int> moveRange;
    [SerializeField] List<Vector2Int> attackRange;
    [SerializeField] List<PassiveType> passives;

    public int CP { get { return cp; } set { cp = value; } }
    public int Movement { get { return movement; } set { Movement = value; } }
    public bool IsKing { get { return isKing; } set { isKing = value; } }
    public List<Vector2Int> MoveRange { get { return moveRange; } set { moveRange = value; } }
    public List<Vector2Int> AttackRange { get { return attackRange; } set { attackRange = value; } }
    public List<PassiveType> Passive { get { return passives; } set { passives = value; } }
    public UnitTag Tag { get { return tag; } }
}
