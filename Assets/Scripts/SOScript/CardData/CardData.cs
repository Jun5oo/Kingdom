using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 카드 한 장의 정적 데이터를 보관하는 ScriptableObject.
/// ID, 이름, 종족, 레벨별 CP, 공격/이동 RangeType, 액션/패시브 타입 목록을 포함한다.
/// CSV 임포터(ReadCSVFile)로 자동 생성되거나 에디터에서 직접 작성할 수 있다.
/// </summary>
[CreateAssetMenu(fileName = "CardData", menuName = "Card Scriptable")]
public class CardData : ScriptableObject
{
    [SerializeField] string id;
    public string ID { get { return id; } internal set { id = value; } }

    [SerializeField] string cardName;
    public string CardName { get { return cardName; } internal set { cardName = value; } }

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
    List<ActionType> action = new List<ActionType>();
    public List<ActionType> Action { get {  return action; } internal set {  action = value; } }

    [SerializeField]
    List<PassiveType> passives = new List<PassiveType>();
    public List<PassiveType> Passive { get { return passives; } internal set { passives = value; } }
}
