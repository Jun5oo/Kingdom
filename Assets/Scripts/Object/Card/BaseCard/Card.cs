using System.Collections.Generic;
using UnityEngine;

public abstract class Card : BaseObject
{
    int ownerID;
    public override int OwnerID { get { return ownerID; } }

    public virtual void Init(CardData cardData, int playerID)
    {
        base.Init(cardData);
        ownerID = playerID; 
    }
}
