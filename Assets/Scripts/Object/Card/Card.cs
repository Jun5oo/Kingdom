using UnityEngine;

/// <summary>
/// 플레이어 핸드에 있는 카드 오브젝트.
/// 소환 전 단계에서 CardData의 스탯을 보관하며, 선택/이동 인터랙션을 CardInteraction에 위임한다.
/// </summary>
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

    /// <summary> 레벨 1 기준으로 스탯을 초기화하고 Interaction/Movement 컴포넌트를 설정한다. </summary>
    public override void Init(CardData cardData, int playerID)
    {
        base.Init(cardData, playerID);

        this.currentLevel = 1;
        this.currentCP = MAXCP;

        interaction.Init(this);
        movement.Init();
    }
}
