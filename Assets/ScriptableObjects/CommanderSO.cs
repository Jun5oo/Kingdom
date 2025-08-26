using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable/CommanderSO")]
public class CommanderSO : ScriptableObject
{
    [SerializeField] public CardData undead;
    [SerializeField] public CardData celestial;
}

