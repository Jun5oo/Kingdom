using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 카드 데이터 스크립트 오브젝트 
/// </summary>

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
