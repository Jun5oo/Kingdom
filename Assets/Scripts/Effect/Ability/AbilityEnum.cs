public enum AbilityTrigger
{
    None, 

    // 필드 위에서 Ability를 실행시켰을 때 
    Activate, 

    // 카드를 냈을 때 
    OnPlay, 

    // 턴이 시작했을 때 
    StartTurn, 
    // 턴이 종료되었을 때 
    EndTurn, 

    // 공격했을 때 
    OnAttackBegin,
    // 공격이 끝났을 때 
    OnAttackEnd, 

    // 적을 처치했을 때 
    OnKill, 

    // 자신이 죽었을 때 
    OnDeath, 
    // 자신을 제외한 다른 유닛이 죽었을 때 
    OnDeathOther, 
    
}

public enum AbilityTarget
{
    None, 
    Self, 

    SelectTarget, 

    LastSummoned, 
    LastTargeted, 
    LastDestroyed, 
}

public enum ConditionOperatorInt
{
    Equal, 
    NotEqual, 
    Less,
    LessEqual, 
    Greater,
    GreaterEqual, 
}

public enum ConditionOperatorBool
{
    True, 
    False, 
}