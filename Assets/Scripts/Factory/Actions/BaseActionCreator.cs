using System.Collections.Generic;

public abstract class BaseActionCreator
{
    public abstract bool CanHandle(BaseObject baseObject);
    public abstract List<ActionType> GetAvailableActions(BaseObject baseObject);
    public abstract IGameAction CreateAction(ActionType gameAction, BaseObject baseObject); 
}
