using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AIResurrectionStrategy
{
    private ActionFactory actionFactory;
    private TokenManager tokenManager;
    private AbilityResourceSystem abilitySystem;

    public AIResurrectionStrategy(ActionFactory actionFactory, TokenManager tokenManager, AbilityResourceSystem abilitySystem)
    {
        this.actionFactory = actionFactory;
        this.tokenManager = tokenManager;
        this.abilitySystem = abilitySystem;
    }

    public async UniTask ResurrectionUnit(ResurrectionAction resurrection, Vector2Int validGirdPos)
    {
        await resurrection.Execute(validGirdPos);
    }

    public bool CanResurrectionAction(int currentPlayerID, out ResurrectionAction resurrectionAction, out Vector2Int validGridPosForResurrection)
    {
        List<Token> tokens = tokenManager.GetTokens(currentPlayerID);
        List<Token> graveyardTokens = tokens.Where(t => t.UnitData.Tag == UnitTag.Graveyard).ToList();

        if (tokenManager.TryGetKingTokenFrom(currentPlayerID, out Token kingToken))
        {
            resurrectionAction = actionFactory.CreateAction(ActionType.Resurrection, kingToken, ActionPerformer.System) as ResurrectionAction;
        }
        else
        {
            resurrectionAction = null;
            validGridPosForResurrection = Vector2Int.zero;
            return false;
        }

        if (graveyardTokens.Count == 0)
        {
            resurrectionAction = null;
            validGridPosForResurrection = Vector2Int.zero;
            return false;
        }

        Token randomToken = GetRandomToken(graveyardTokens);
        validGridPosForResurrection = tokenManager.GetGridPositionOfToken(randomToken);

        return abilitySystem.IsEnoughResources(currentPlayerID, resurrectionAction.Cost);
    }

    private Token GetRandomToken(List<Token> tokens)
    {
        int randomIndex = UnityEngine.Random.Range(0, tokens.Count);
        Token token = tokens[randomIndex];
        return token;
    }
}
