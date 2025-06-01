using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CardData", menuName = "Card Scriptable")]
public class CardData : ScriptableObject
{
    public string cardName;
    public int level; 
    public int cp; // combat power
    public bool isKing = false;

    public string description;

    public List<Vector2Int> moveRange;
    public List<Vector2Int> attackRange;

    public int movement; 

    public Sprite sprite;

    public List<ActionType> actions; 
}
