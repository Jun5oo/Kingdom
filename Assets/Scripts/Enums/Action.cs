/// <summary>카드/토큰이 수행할 수 있는 행동 종류.</summary>
public enum ActionType
{
    None, 
    Summon, 
    Move, 
    Attack,
    Resurrection, 
    DivineShield, 
    Upgrade, 
}

public enum ActionPerformer
{
    System,
    Player
}

// TODO: 각 ActionState 수정 혹은 통일 필요.
public enum SummonState
{
    Prepare,
    Summon,
    Placing,
    Done
}

public enum MoveState
{
    Prepare,
    Animation,
    Placing,
    Done
}

public enum AttackState
{
    Prepare,
    Attack,
    Placing,
    Done
}

