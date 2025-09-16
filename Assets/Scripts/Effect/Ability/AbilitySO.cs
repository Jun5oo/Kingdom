using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Ability/AbilitySO")]
public class AbilitySO : ScriptableObject
{
    public string abilityName;
    [TextArea(5, 7)]
    public string abilityDescription;

    [SerializeField]
    public List<TriggeredEffect> triggeredEffects; 
}
