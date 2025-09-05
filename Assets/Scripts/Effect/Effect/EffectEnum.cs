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
    Active, 
    OnAllyDead, 
    OnEnemyDead, 
    OnTurnStarted, 
    OnTurnEnded,
    OnUnitDead, 
}

public enum Target
{
    None, 
    Self, 
    Board, 
    Select, 
    Ally,
    AllAllies, 
    Enemy, 
    AllEnemies  
}

public enum BuffType
{
    DivineShield
}


public enum SelectionMode 
{
    Default, 
    Select, 
    Random 
}