using System.Collections.Generic;
using UnityEngine;

public abstract class Card : BaseObject
{
    int ownerID; 

    public virtual void Init(CardData cardData, int playerID)
    {
        base.Init(cardData);
        ownerID = playerID; 
    }

    public override int OwnerID {  get { return ownerID; } }
}
