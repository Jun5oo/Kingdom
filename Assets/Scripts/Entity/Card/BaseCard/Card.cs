using System.Collections.Generic;
using UnityEngine;

public abstract class Card : Entity
{
    protected int ownerPlayerID;

    public abstract CardData CardData { get; }
    public override string Name { get { return CardData.Name; } }
    public override Sprite Sprite { get { return CardData.Sprite; } }
    public override string Description { get {  return CardData.Description; } }
    public override int OwnerPlayerID { get { return ownerPlayerID; } }
    public abstract void Init(CardData cardData, int playerID);
}
