using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitCardData", menuName = "Card Scriptable/Unit")]
public class UnitCardData : CardData
{
    [SerializeField] int cp;
    [SerializeField] int movement;
    [SerializeField] bool isKing;

    [SerializeField] List<Vector2Int> moveRange;
    [SerializeField] List<Vector2Int> attackRange;

    // 추후에 수정 필요 
    [SerializeField] List<PassiveType> passives;

    public int CP { get { return cp; } }
    public int Movement { get { return movement; } }
    public bool IsKing { get { return isKing; } }
    public List<Vector2Int> MoveRange { get { return moveRange; } }
    public List<Vector2Int> AttackRange { get { return attackRange; } }

    public List<PassiveType> Passive {  get { return passives; } }
    
}
