using System.Collections.Generic;

public class ActionResolver
{
    public List<ActionType> GetValidActions(BaseObject baseObject)
    {
        var actions = new List<ActionType>();

        switch (baseObject)
        {
            case Card card:
                actions.Add(ActionType.Summon);
                break;
            case Token token:
                if(token.Data.MoveType[token.Level-1] != RangeType.None)
                    actions.Add(ActionType.Move);
                if (token.Data.AttackType[token.Level-1] != RangeType.None)
                    actions.Add(ActionType.Attack);
                if (token.Data.Tag == UnitTag.King)
                    actions.Add(ActionType.Upgrade); 
                if(token.Abilities.Count > 0)
                {
                    // TODO 
                    // Ability가 Active type인 경우, ActionType.Ability를 추가하도록 해야한다. 
                }
                break; 
        }

        return actions; 
    }

}
