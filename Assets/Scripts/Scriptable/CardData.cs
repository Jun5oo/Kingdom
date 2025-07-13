using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CardData", menuName = "Card Scriptable")]
public class CardData : ScriptableObject
{
    [SerializeField] string cardName;
    [SerializeField] int level;
    [SerializeField] Sprite sprite;
    [SerializeField] int cp;
    [SerializeField] int movement;
    [SerializeField] string description;
    [SerializeField] bool isKing;
    [SerializeField] List<ActionType> actions;
    [SerializeField] List<Vector2Int> moveRange;
    [SerializeField] List<Vector2Int> attackRange;

    public string Name {  get { return cardName; } }
    public int Level { get { return level; } }
    public int CP {  get { return cp; } }
    public int Movement { get { return movement; } }
    public Sprite Sprite { get { return sprite; } }
    public string Description { get { return description; } }
    public bool IsKing { get {  return isKing; } }
    public List<ActionType> Actions { get {  return actions; } }
    public List<Vector2Int> MoveRange { get {  return moveRange; } }
    public List<Vector2Int> AttackRange { get { return attackRange; } }
}
