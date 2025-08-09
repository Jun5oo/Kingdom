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

    public int ID { get { return cardID; } }
    public string Name {  get { return cardName; } }
    public string Description { get { return description; } }
    public Race Race { get { return race; } }
    public List<ActionType> Actions { get { return actions; } }
}
