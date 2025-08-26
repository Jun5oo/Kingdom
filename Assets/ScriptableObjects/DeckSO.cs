using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable/DeckData")]
public class DeckSO : ScriptableObject
{
    [SerializeField] public List<CardData> undeadDeck;
    [SerializeField] public List<CardData> celestialDeck;
}

