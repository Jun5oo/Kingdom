using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class AIAttackStrategy
{
    private ActionFactory actionFactory;
    private TokenManager tokenManager;

    public AIAttackStrategy(ActionFactory actionFactory, TokenManager tokenManager)
    {
        this.actionFactory = actionFactory;
        this.tokenManager = tokenManager;
    }

    public async UniTask AttackRandomTarget(AttackAction attackAction, List<Vector2Int> validGridPosListForAttack)
    {
        int randomIndex = UnityEngine.Random.Range(0, validGridPosListForAttack.Count);
        Vector2Int randomGridPos = validGridPosListForAttack[randomIndex];
        await attackAction.Execute(randomGridPos);
    }

    public bool CanAttackAction(int currentPlayerID, out AttackAction attackAction, out List<Vector2Int> validGridPosList)
    {
        List<Token> tokens = tokenManager.GetTokens(currentPlayerID);
        Token randomToken = GetRandomToken(tokens);

        attackAction = actionFactory.CreateAction(ActionType.Attack, randomToken) as AttackAction;

        List<Vector2Int> tempListToRemove = new List<Vector2Int>();

        Vector2Int tokenPos = tokenManager.GetGridPositionOfToken(randomToken);

        validGridPosList = attackAction.AttackablePositions.ToList();

        for (int i = 0; i < validGridPosList.Count; i++)
        {
            validGridPosList[i] += tokenPos;
        }

        foreach (var tempAttackablePos in validGridPosList)
        {
            // 보드 외부 검사
            if (tempAttackablePos.y < 0 || tempAttackablePos.y >= GridManager.WIDTH || tempAttackablePos.x < 0 || tempAttackablePos.x >= GridManager.HEIGHT)
            {
                tempListToRemove.Add(tempAttackablePos);
                continue;
            }

            // 토큰 검사
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
