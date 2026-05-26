using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// AI의 이동 행동을 결정하는 전략 클래스.
/// 무작위 토큰을 선택하고 이동 가능한 빈 위치(보드 내, 토큰 없음)를 필터링한다.
/// </summary>
public class AIMoveStrategy
{
    private ActionFactory actionFactory;
    private TokenManager tokenManager;

    public AIMoveStrategy(ActionFactory actionFactory, TokenManager tokenManager)
    {
        this.actionFactory = actionFactory;
        this.tokenManager = tokenManager;
    }

    /// <summary> validGirdPos 중 무작위 위치를 선택하여 이동을 실행한다. </summary>
    public async UniTask MoveRandomPos(MoveAction move, List<Vector2Int> validGirdPos)
    {
        int randomIndex = UnityEngine.Random.Range(0, validGirdPos.Count);
        Vector2Int randomGridPos = validGirdPos[randomIndex];
        await move.Execute(randomGridPos);
    }

    /// <summary>
    /// 무작위 토큰으로 이동 가능한 위치 목록을 계산한다.
    /// 보드 외부이거나 다른 토큰이 있는 위치는 제외한다.
    /// 유효 위치가 없으면 false를 반환한다.
    /// </summary>
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
            if (tempMoveablePos.y < 0 || tempMoveablePos.y >= GridManager.WIDTH || tempMoveablePos.x < 0 || tempMoveablePos.x >= GridManager.HEIGHT)
            {
                tempListToRemove.Add(tempMoveablePos);
                continue;
            }

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
