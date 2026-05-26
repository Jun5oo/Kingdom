using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// AI의 공격 행동을 결정하는 전략 클래스.
/// 무작위 토큰을 선택하고 공격 가능한 적 위치(보드 내, 적 소유 토큰)를 필터링한다.
/// </summary>
public class AIAttackStrategy
{
    private ActionFactory actionFactory;
    private TokenManager tokenManager;

    public AIAttackStrategy(ActionFactory actionFactory, TokenManager tokenManager)
    {
        this.actionFactory = actionFactory;
        this.tokenManager = tokenManager;
    }

    /// <summary> validGridPosListForAttack 중 무작위 위치를 선택하여 공격을 실행한다. </summary>
    public async UniTask AttackRandomTarget(AttackAction attackAction, List<Vector2Int> validGridPosListForAttack)
    {
        int randomIndex = UnityEngine.Random.Range(0, validGridPosListForAttack.Count);
        Vector2Int randomGridPos = validGridPosListForAttack[randomIndex];
        await attackAction.Execute(randomGridPos);
    }

    /// <summary>
    /// 무작위 토큰으로 공격 가능한 위치 목록을 계산한다.
    /// 보드 외부이거나 적 토큰이 없는 위치는 제외한다.
    /// 유효 위치가 없으면 false를 반환한다.
    /// </summary>
    public bool CanAttackAction(int currentPlayerID, out AttackAction attackAction, out List<Vector2Int> validGridPosList)
    {
        List<Token> tokens = tokenManager.GetTokens(currentPlayerID);
        Token randomToken = GetRandomToken(tokens);

        attackAction = actionFactory.CreateAction(ActionType.Attack, randomToken, ActionPerformer.System) as AttackAction;

        List<Vector2Int> tempListToRemove = new List<Vector2Int>();

        Vector2Int tokenPos = tokenManager.GetGridPositionOfToken(randomToken);

        validGridPosList = attackAction.AttackablePositions.ToList();

        for (int i = 0; i < validGridPosList.Count; i++)
        {
            validGridPosList[i] += tokenPos;
        }

        foreach (var tempAttackablePos in validGridPosList)
        {
            if (tempAttackablePos.y < 0 || tempAttackablePos.y >= GridManager.WIDTH || tempAttackablePos.x < 0 || tempAttackablePos.x >= GridManager.HEIGHT)
            {
                tempListToRemove.Add(tempAttackablePos);
                continue;
            }

            if (!tokenManager.TryGetTokenFrom(tempAttackablePos, out Token token) || token.OwnerID == currentPlayerID)
            {
                tempListToRemove.Add(tempAttackablePos);
                continue;
            }
        }

        foreach (var tempGridPos in tempListToRemove)
        {
            validGridPosList.Remove(tempGridPos);
        }

        if (validGridPosList.Count == 0)
        {
            Debug.Log("유효한 그리드가 없습니다. 소환을 실행할 수 없습니다.");
            return false;
        }

        return true;
    }

    private Token GetRandomToken(List<Token> tokens)
    {
        int randomIndex = UnityEngine.Random.Range(0, tokens.Count);
        Token token = tokens[randomIndex];
        return token;
    }
}
