using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// AI의 부활 능력 사용을 결정하는 전략 클래스.
/// 필드 위 무덤(Graveyard) 토큰 중 하나를 무작위로 선택하여 부활시킨다.
/// 어빌리티 코인이 부족하거나 무덤이 없으면 false를 반환한다.
/// </summary>
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
        List<Token> graveyardTokens = tokens.Where(t => t.Data.Tag == UnitTag.Graveyard).ToList();

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
