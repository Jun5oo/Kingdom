using System.Collections.Generic;
using UnityEngine;

public struct UnitDeadEvent : IGameEvent
{
    public Token killer;
    public Token victim;

    public int killerOwnerID; 
    public int victimOwnerID;   

    public Vector2Int killerPosition;
    public Vector2Int victimPosition;

    public List<CardData> killerSources;
    public List<CardData> victimSources;
}
