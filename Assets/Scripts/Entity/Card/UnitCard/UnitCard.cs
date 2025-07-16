using NUnit.Framework.Constraints;
using System.Collections.Generic;
using UnityEngine;

public class UnitCard : Card, IUnit
{
    [Header("Original Data")]
    UnitCardData unitCardData;

    [Header("RunTime Data")]
    [SerializeField] int currentCP;

    [Header("Components")]
    [SerializeField] CardView cardView;
    [SerializeField] CardHover cardHover;
    [SerializeField] CardMovement cardMovement;

    public override CardData CardData { get { return unitCardData; } }
    public override List<ActionType> Actions { get { return UnitCardData.Actions; } }

    public UnitCardData UnitCardData { get { return unitCardData; } }

    public int CP { get { return currentCP; } }
    public int CurrentMovement { get { return UnitCardData.Movement; } }
    public bool IsKing { get { return UnitCardData.IsKing; } }
    public List<Vector2Int> MoveableRange { get { return UnitCardData.MoveRange; } }
    public List<Vector2Int> AttackRange { get { return UnitCardData.AttackRange; } }

    public override void Init(CardData cardData, int playerID)
    {
        if (cardData is not UnitCardData unitCardData)
            throw new System.Exception("잘못된 CardData 전달됨"); 

        this.unitCardData = unitCardData;
        this.currentCP = unitCardData.CP; 
        this.ownerPlayerID = playerID;

        cardView.Init(UnitCardData.CardArt);
        cardHover?.Init();
    }
}
