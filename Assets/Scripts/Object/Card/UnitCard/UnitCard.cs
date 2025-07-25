using System.Collections.Generic;
using UnityEngine;

public class UnitCard : Card
{
    [Header("RunTime Data")]
    [SerializeField] int currentCP;
    
    [Header("Components")]
    [SerializeField] CardView view;
    [SerializeField] CardInteraction interaction;
    [SerializeField] CardMovement movement;

    public UnitCardData UnitData { get { return Data as UnitCardData; } }

    public int CP { get { return currentCP; } }
    public int MAXCP { get { return UnitData.CP; } }
    public int Movement { get { return UnitData.Movement; } }
    public bool IsKing { get { return UnitData.IsKing; } }
    public List<Vector2Int> MoveableRange { get { return UnitData.MoveRange; } }
    public List<Vector2Int> AttackRange { get { return UnitData.AttackRange; } }

    public override void Init(CardData unitCardData, int playerID)
    {
        base.Init(unitCardData, playerID);

        this.currentCP = CP;

        Debug.Log($"[UnitCard] Init called with data: {unitCardData?.Name}");

        interaction.Init(this);
        movement.Init();
    }
}
