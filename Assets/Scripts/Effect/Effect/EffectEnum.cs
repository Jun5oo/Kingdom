public enum EffectType
{
    Damage, 
    Summon,
    Resurrect, 
    Buff, 
    Gain,
    Destroy,
    Draw,
}

public enum Trigger 
{
    None, 
    
    Active,
    
    OnTurnStarted,
    OnTurnEnded,

    OnUnitDead,
}

public enum Target
{
    None, 
    // 시전자 
    Self, 
    // 보드 위의 유닛 대상
    Board, 
    // 선택한 유닛 
    Select, 

    // 처치한 유닛 
    Kill, 
    // 죽은 유닛 
    Death, 

    // 마지막으로 파괴된 유닛 
    LastDestroyed, 
}

public enum BuffType
{
    DivineShield
}
