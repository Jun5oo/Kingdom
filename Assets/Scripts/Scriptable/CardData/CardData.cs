using UnityEngine;

[CreateAssetMenu(fileName = "CardData", menuName = "Card Scriptable")]
public class CardData : ScriptableObject
{
    public string heroName;
    public int attack;
    public int hp;
    public int attackRange;

    public GameObject testObject; 
}
