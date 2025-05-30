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

    // 공격패턴, 움직임의 패턴에 다양성을 줄 것을 생각한다면 List<Vector2Int>를, 단순히 직선 방향의 공격만을 채택한다면 int값을 가져와서 다루는 것도 고려. 
    public List<Vector2Int> moveRange;
    public List<Vector2Int> attackRange;

    public int movement; 

    public Sprite sprite; 
}
