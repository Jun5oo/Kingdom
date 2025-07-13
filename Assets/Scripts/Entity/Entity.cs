
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour 
{
    public abstract string Name { get; }
    public abstract Sprite Sprite { get; }
    public abstract string Description { get; }
    public abstract List<ActionType> Actions { get; }
    public abstract int OwnerPlayerID { get; }

    // 현재는 Spell 카드가 존재하지 않으므로 (Unit 카드만 존재하므로) CP, Movement의 경우 반드시 추가하도록 함 
    // 추후 카드의 종류를 추가한다면 CombatEntity, SpellEntity로 나눌 예정. 

    public abstract int Level { get; }
    public abstract int CP { get; } 
    public abstract int Movement { get; }
}

