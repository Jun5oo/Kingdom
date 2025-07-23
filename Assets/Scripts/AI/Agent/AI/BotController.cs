using UnityEngine;

public class BotController : AgentController
{
    public override void InitializePhaseStates()
    {
    }

    /// <summary>
    /// 0~2턴 1레벨, 3~5턴 2레벨, 6턴 이후 3레벨  
    /// </summary>
    /// 
    public int GetDesiredLevel(int turnCount)
    {
        return Mathf.Min(turnCount / 3 + 1, 3);
    }

}
