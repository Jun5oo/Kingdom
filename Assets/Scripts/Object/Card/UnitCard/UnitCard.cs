using System.Collections.Generic;
using UnityEngine;

public class UnitCard : Card
{
    [Header("RunTime Data")]
    [SerializeField] int currentLevel;
    [SerializeField] int currentCP;
    
    [Header("Components")]
    [SerializeField] CardView view;
    [SerializeField] CardInteraction interaction;
    [SerializeField] CardMovement movement;

    public UnitCardData UnitData { get { return Data as UnitCardData; } }

    public int CP { get { return currentCP; } }
    public int MAXCP { get { return UnitData.GetCP(currentLevel); } }
    public int Movement { get { return UnitData.GetMovement(currentLevel); } }
    public int Level { get { return currentLevel; } }

    public UnitTag Tag { get { return UnitData.Tag; } } 

    public List<Vector2Int> MoveableRange { get { return UnitData.MoveRange; } }
    public List<Vector2Int> AttackRange { get { return UnitData.AttackRange; } }

    public override void Init(CardData unitCardData, int playerID)
    {
        base.Init(unitCardData, playerID);

        this.currentLevel = 1; 
        this.currentCP = UnitData.GetCP(1); 

        interaction.Init(this);
        movement.Init();
    }
}
