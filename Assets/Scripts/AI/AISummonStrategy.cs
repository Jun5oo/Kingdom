using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;

/// <summary>
/// AI의 소환 행동을 결정하는 전략 클래스.
/// 핸드에서 무작위 카드를 선택하고, 왕 기준 인접 위치에서 유효한 소환 위치를 필터링한다.
/// </summary>
public class AISummonStrategy
{
    private HandManager handManager;
    private ActionFactory actionFactory;
    private TokenManager tokenManager;

    public AISummonStrategy(HandManager playerHandManager, ActionFactory actionFactory, TokenManager tokenManager)
    {
        this.handManager = playerHandManager;
        this.actionFactory = actionFactory;
        this.tokenManager = tokenManager;
    }

    /// <summary> validGirdPos 중 무작위 위치를 선택하여 소환을 실행한다. </summary>
    public async UniTask SummonRandomPos(SummonAction summon, List<Vector2Int> validGirdPos)
    {
        int randomIndex = UnityEngine.Random.Range(0, validGirdPos.Count);
        Vector2Int randomGridPos = validGirdPos[randomIndex];
        await summon.Execute(randomGridPos);
    }

    /// <summary>
    /// 핸드에서 무작위 카드로 소환 가능한 위치 목록을 계산한다.
    /// 왕 위치 기준으로 ValidPositions에 오프셋을 더하고 보드 외부·토큰 점유 위치를 제외한다.
    /// 유효 위치가 없으면 false를 반환한다.
    /// </summary>
    public bool CanSummonAction(int currentPlayerID, out SummonAction summonAction, out List<Vector2Int> validGridPosList)
    {
        List<Card> cards = handManager.GetHandCardsList(currentPlayerID);

        if (cards.Count == 0)
        {
            summonAction = null;
            validGridPosList = null;
            return false;
        }

        Card card = GetRandomCard(cards);

        summonAction = actionFactory.CreateAction(ActionType.Summon, card, ActionPerformer.System) as SummonAction;

        validGridPosList = summonAction.ValidPositions.ToList();

        if (summonAction.TryGetKingTokenPos(out Vector2Int gridPos))
        {
            for (int i = 0; i < validGridPosList.Count; i++)
            {
                validGridPosList[i] += gridPos;
            }

            List<Vector2Int> tempListToRemove = new List<Vector2Int>();

            foreach (var tempValidPos in validGridPosList)
            {
                if (tempValidPos.y < 0 || tempValidPos.y >= GridManager.WIDTH || tempValidPos.x < 0 || tempValidPos.x >= GridManager.HEIGHT)
                {
                    tempListToRemove.Add(tempValidPos);
                    continue;
                }

                if (tokenManager.TryGetTokenFrom(tempValidPos, out Token token))
                {
                    tempListToRemove.Add(tempValidPos);
                    continue;
                }
            }

            foreach (var tempGridPos in tempListToRemove)
            {
                validGridPosList.Remove(tempGridPos);
            }
        }

        if (validGridPosList.Count == 0)
        {
            Debug.Log("유효한 그리드가 없습니다. 소환을 실행할 수 없습니다.");
            return false;
        }

        return true;
    }

    private Card GetRandomCard(List<Card> cards)
    {
        int randomIndex = UnityEngine.Random.Range(0, cards.Count);
        Card card = cards[randomIndex];
        return card;
    }
}