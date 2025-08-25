using UnityEngine;

public struct UnitDeadEvent : IGameEvent
{
    public Token killer;
    public Token victim;

    public Vector2Int killerPosition;
    public Vector2Int victimPosition; 
}
