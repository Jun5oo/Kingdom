using System.Collections.Generic;

public class CardActionCreator : BaseActionCreator
{
    public override bool CanHandle(BaseObject baseObject)
    {
        return baseObject is Card; 
    }

    public override IGameAction CreateAction(ActionType gameAction, BaseObject baseObject)
    {
        if (!(baseObject is Card card))
            return null;

        switch (gameAction)
        {
            case ActionType.Summon:
                return new SummonAction(card);

            default:
                return null; 
        }
    }

    public override List<ActionType> GetAvailableActions(BaseObject baseObject)
    {
        var gameActions = new List<ActionType>();

        if (baseObject is Card card)
            gameActions.Add(ActionType.Summon);

        return gameActions;
    }
}
