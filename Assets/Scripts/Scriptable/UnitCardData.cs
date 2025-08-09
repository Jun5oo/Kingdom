using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitCardData", menuName = "Card Scriptable/Unit")]
public class UnitCardData : CardData
{
    // 레벨 
    [SerializeField] int level; 
    // 등급 별 CP 
    [SerializeField] List<int> levelCP;
    // 등급 별 Movement 
    [SerializeField] List<int> movement;

    // 추후 IsKing이 아닌 Tag로 교체 예정     
    [SerializeField] UnitTag tag;
    [SerializeField] bool isKing;

    // 공격가능한 벡터 (사거리) 
    [SerializeField] List<Vector2Int> attackRange;
    // 이동가능한 벡터 (이동거리) 
    [SerializeField] List<Vector2Int> moveRange;
    // 보유한 패시브 스킬 
    [SerializeField] List<PassiveType> passives;

    public int Level { get { return level; } }
    public List<int> CP { get { return levelCP; } set { levelCP = value; } }
    public List<int> Movement { get { return movement; } set { Movement = value; } }

    public UnitTag Tag { get { return tag; } }
    public bool IsKing { get { return isKing; } set { isKing = value; } }
    
    public List<Vector2Int> MoveRange { get { return moveRange; } set { moveRange = value; } }
    public List<Vector2Int> AttackRange { get { return attackRange; } set { attackRange = value; } }
    
    public List<PassiveType> Passive { get { return passives; } set { passives = value; } }

    public int GetCP(int level) => GetStatus(CP, level);
    public int GetMovement(int level) => GetStatus(Movement, level); 
    public int GetStatus(List<int> list, int level)
    {
        if(level < 1 || list.Count < level)
        {
            Debug.LogError($"{level}레벨은 해당 유닛에게 존재하지 않는 레벨 입니다.");
            return 0; 
        }

        return list[level - 1]; 
    }
}
