
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : MonoBehaviour 
{
    public virtual string Name { get; }
    public virtual Sprite Sprite { get; }
    public virtual string Description { get; }
    public virtual int OwnerPlayerID { get; }
    public virtual List<ActionType> Actions { get; }
}

