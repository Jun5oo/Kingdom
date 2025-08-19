public enum EffectType
{
    Damage, 
    Summon, 
    Buff, 
    Gain,
    Destroy,
    Draw,
}

public enum Trigger 
{
    // 발동했을 때
    Active, 
    // 소환되었을 때
    OnPlay, 
    OnAllyDead, 
    OnEnemyDead, 
    OnTurnStarted, 
    OnTurnEnded 
}

public enum Target
{
    None, 
    Self, 
    Ally,
    AllAllies, 
    Enemy, 
    AllEnemies  
}

public enum BuffType
{
    DivineShield
}