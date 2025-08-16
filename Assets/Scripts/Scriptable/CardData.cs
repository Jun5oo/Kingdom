using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CardData", menuName = "Card Scriptable")]
public abstract class CardData : ScriptableObject
{
    [SerializeField] int cardID; 
    [SerializeField] string cardName;
    [SerializeField] string description;
    [SerializeField] Race race; 

    [SerializeField] List<ActionType> actions;

    public int ID { get { return cardID; } set { cardID = value; } }
    public string Name {  get { return cardName; } set { cardName = value; } }
    public string Description { get { return description; } set { description = value; } }
    public Race Race { get { return race; } set { race = value; } }
    public List<ActionType> Actions { get { return actions; } set { actions = value; } }
}
