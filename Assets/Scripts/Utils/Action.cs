public enum ActionType
{
    Summon, 
    Move, 
    Attack,
    Resurrection, 
    DivineShield
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
    Animation,
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
