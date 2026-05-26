using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// AI의 신성방패 능력 사용을 결정하는 전략 클래스.
/// 왕 토큰을 찾아 DivineShieldAction을 생성하고, 무작위 아군에게 적용한다.
/// 어빌리티 코인이 부족하면 false를 반환한다.
/// </summary>
public class AIDivineShieldStrategy
{
    private ActionFactory actionFactory;
    private TokenManager tokenManager;
    AbilityResourceSystem abilitySystem;

    public AIDivineShieldStrategy(ActionFactory actionFactory, TokenManager tokenManager, AbilityResourceSystem abilitySystem)
    {
        this.actionFactory = actionFactory;
        this.tokenManager = tokenManager;
        this.abilitySystem = abilitySystem;
    }

    public async UniTask DivineShieldUnit(DivineShieldAction divineShield, Vector2Int validGridPos)
    {
        await divineShield.Execute(validGridPos);
    }

    public bool CanDivineShieldAction(int currentPlayerID, out DivineShieldAction divineShieldAction, out Vector2Int validGridPosForDivineShield)
    {
        List<Token> tokens = tokenManager.GetTokens(currentPlayerID).ToList();

        if (tokenManager.TryGetKingTokenFrom(currentPlayerID, out Token kingToken))
        {
            tokens.Remove(kingToken);
            divineShieldAction = actionFactory.CreateAction(ActionType.DivineShield, kingToken, ActionPerformer.System) as DivineShieldAction;
        }
        else
        {
            divineShieldAction = null;
            validGridPosForDivineShield = Vector2Int.zero;
            return false;
        }

        if (tokens.Count == 0)
        {
            divineShieldAction = null;
            validGridPosForDivineShield = Vector2Int.zero;
            return false;
        }

        Token randomToken = GetRandomToken(tokens);
        validGridPosForDivineShield = tokenManager.GetGridPositionOfToken(randomToken);

        return abilitySystem.IsEnoughResources(currentPlayerID, divineShieldAction.Cost);
    }

    private Token GetRandomToken(List<Token> tokens)
    {
        int randomIndex = UnityEngine.Random.Range(0, tokens.Count);
        Token token = tokens[randomIndex];
        return token;
    }
}
