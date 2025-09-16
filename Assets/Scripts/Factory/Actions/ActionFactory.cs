using System.Collections.Generic;
public class ActionFactory
{
    List<BaseActionCreator> creators; 
    public ActionFactory()
    {
        creators = new List<BaseActionCreator>
        {
            new CardActionCreator(), 
            new TokenActionCreator()
        };
    }

    public IGameAction CreateAction(ActionType actionType, BaseObject baseObject)
    {
        var creator = GetActionCreator(baseObject);
        if(creator == null)
            return null; 

        return creator.CreateAction(actionType, baseObject);
    }

    public List<ActionType> GetAvailableActions(BaseObject baseObject)
    {
        var creator = GetActionCreator(baseObject);
        if (creator == null)
            return new List<ActionType>(); 

        return creator.GetAvailableActions(baseObject);
    }

    public BaseActionCreator GetActionCreator(BaseObject baseObject)
    {
        foreach(var creator in creators)
        {
            if (creator.CanHandle(baseObject))
                return creator; 
        }

        return null; 
    }
}
