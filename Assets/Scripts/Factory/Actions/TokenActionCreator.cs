using System;
using System.Collections.Generic;

public class TokenActionCreator : BaseActionCreator
{
    public override bool CanHandle(BaseObject baseObject)
    {
        return baseObject is Token; 
    }

    public override IGameAction CreateAction(ActionType gameAction, BaseObject baseObject)
    {
        if (!(baseObject is Token token))
            return null; 

        switch(gameAction)
        {
            case ActionType.Move:
                return CanMove(token) ? new MoveAction(token) : null;
            case ActionType.Attack:
                return CanAttack(token) ? new AttackAction(token) : null;
            case ActionType.Ability:
                return HasAbility(token) ? CreateAbilityAction(token) : null;
            case ActionType.Upgrade:
                return token.Tag == UnitTag.King ? new UpgradeAction(token) : null;

            default:
                return null; 
        }
    }

    private IGameAction CreateAbilityAction(Token token)
    {
        foreach (var ability in token.Abilities)
        {
            if (ability.AbilityData?.triggeredEffects != null)
            {
                foreach (var binding in ability.AbilityData.triggeredEffects)
                {
                    if (binding.trigger == Trigger.Active)
                        return new AbilityAction(ability, token); 
                }
            }
        }

        return null;
    }

    public override List<ActionType> GetAvailableActions(BaseObject baseObject)
    {
        var gameActions = new List<ActionType>();

        if (!(baseObject is Token token))
            return gameActions;

        if (CanMove(token))
            gameActions.Add(ActionType.Move); 
        if (CanAttack(token))
            gameActions.Add(ActionType.Attack);
        if (HasAbility(token))
            gameActions.Add(ActionType.Ability);

        if (token.Tag == UnitTag.King)
            gameActions.Add(ActionType.Upgrade);
        
        return gameActions;
    }

    bool CanMove(Token token)
    {
        if (token.Data.MoveRange == null || token.Level <= 0)
            return false;

        var moveType = token.Data.MoveType[token.Level - 1];
        return moveType != RangeType.None;
    }

    bool CanAttack(Token token)
    {
        if (token.Data.AttackRange == null || token.Level <= 0)
            return false;

        var attackType = token.Data.AttackType[token.Level - 1];
        return attackType != RangeType.None;
    }

    bool HasAbility(Token token)
    {
        if(token.Abilities == null || token.Abilities.Count == 0) return false;

        foreach(var ability in token.Abilities)
        {
            if(ability.AbilityData?.triggeredEffects != null)
            {
                foreach(var  binding in ability.AbilityData.triggeredEffects)
                {
                    if (binding.trigger == Trigger.Active)
                        return true; 
                }
            }
        }

        return false; 
    }
}
