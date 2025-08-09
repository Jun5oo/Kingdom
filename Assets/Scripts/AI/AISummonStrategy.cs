using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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

    public async UniTask SummonRandomPos(SummonAction summon, List<Vector2Int> validGirdPos)
    {
        int randomIndex = UnityEngine.Random.Range(0, validGirdPos.Count);
        Vector2Int randomGridPos = validGirdPos[randomIndex];
        await summon.Execute(randomGridPos);
    }

    public bool CanSummonAction(int currentPlayerID, out SummonAction summonAction, out List<Vector2Int> validGridPosList)
    {
        List<Card> cards = handManager.GetHandCards(currentPlayerID);
        // 테스트

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
                // 보드 외부 검사
                if (tempValidPos.y < 0 || tempValidPos.y >= GridManager.WIDTH || tempValidPos.x < 0 || tempValidPos.x >= GridManager.HEIGHT)
                {
                    tempListToRemove.Add(tempValidPos);
                    continue;
                }

                // 토큰 검사
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

    private static Card GetRandomCard(List<Card> cards)
    {
        int randomIndex = UnityEngine.Random.Range(0, cards.Count);
        Card card = cards[randomIndex];
        return card;
    }
}