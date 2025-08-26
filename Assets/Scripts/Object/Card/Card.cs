using UnityEngine;

public class Card : BaseObject
{
    [Header("RunTime Data")]
    [SerializeField] int currentLevel;
    [SerializeField] int currentCP;

    [Header("Components")]
    [SerializeField] CardView view;
    [SerializeField] CardInteraction interaction;
    [SerializeField] CardMovement movement;

    public int CP { get { return currentCP; } }
    public int MAXCP { get { return Data.CP[currentLevel - 1]; } }
    public int Movement { get { return Data.MoveRange[currentLevel - 1]; } }
    public int Level { get { return currentLevel; } }
    public UnitTag Tag { get { return Data.Tag; } }

    public override void Init(CardData cardData, int playerID)
    {
        base.Init(cardData, playerID);

        this.currentLevel = 1;
        this.currentCP = MAXCP;

        interaction.Init(this);
        movement.Init();
    }
}
