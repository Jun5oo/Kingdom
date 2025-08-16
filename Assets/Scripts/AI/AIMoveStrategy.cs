using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AIMoveStrategy
{
    private ActionFactory actionFactory;
    private TokenManager tokenManager;

    public AIMoveStrategy(ActionFactory actionFactory, TokenManager tokenManager)
    {
        this.actionFactory = actionFactory;
        this.tokenManager = tokenManager;
    }

    public async UniTask MoveRandomPos(MoveAction move, List<Vector2Int> validGirdPos)
    {
        int randomIndex = UnityEngine.Random.Range(0, validGirdPos.Count);
        Vector2Int randomGridPos = validGirdPos[randomIndex];
        await move.Execute(randomGridPos);
    }

    public bool CanMoveAction(int currentPlayerID, out MoveAction moveAction, out List<Vector2Int> validGridPosList)
    {
        List<Token> tokens = tokenManager.GetTokens(currentPlayerID);
        Token randomToken = GetRandomToken(tokens);

        moveAction = actionFactory.CreateAction(ActionType.Move, randomToken, ActionPerformer.System) as MoveAction;

        List<Vector2Int> tempListToRemove = new List<Vector2Int>();

        Vector2Int tokenPos = tokenManager.GetGridPositionOfToken(randomToken);

        validGridPosList = moveAction.MoveablePositions.ToList();

        for (int i = 0; i < validGridPosList.Count; i++)
        {
            validGridPosList[i] += tokenPos;
        }

        foreach (var tempMoveablePos in validGridPosList)
        {
            // 보드 외부 검사
            if (tempMoveablePos.y < 0 || tempMoveablePos.y >= GridManager.WIDTH || tempMoveablePos.x < 0 || tempMoveablePos.x >= GridManager.HEIGHT)
            {
                tempListToRemove.Add(tempMoveablePos);
                continue;
            }

            // 토큰 검사
            if (tokenManager.TryGetTokenFrom(tempMoveablePos, out Token token))
            {
                tempListToRemove.Add(tempMoveablePos);
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
