using Cysharp.Threading.Tasks;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 토큰 소환 공통 로직을 담당하는 시스템.
/// SummonAction, SummonGraveyard(패시브), ResurrectionAction 등 소환이 필요한 모든 곳에서 사용한다.
/// </summary>
public class SummonSystem
{
    TokenManager tokenManager;
    TokenFactory tokenFactory;
    GridManager gridManager;

    public void Init()
    {
        tokenManager = ServiceLocator.Get<TokenManager>();
        tokenFactory = ServiceLocator.Get<TokenFactory>();
        gridManager = ServiceLocator.Get<GridManager>();
    }

    /// <summary>
    /// 지정한 그리드 위치에 토큰을 생성하고 낙하 애니메이션을 재생한다.
    /// sourceObject: 이 토큰을 생성한 주체(예: 무덤을 만든 언데드 왕)
    /// sourceObjects: 업그레이드 재료 목록
    /// spawnLevel: 소환 시 레벨 (기본 1)
    /// </summary>
    public async UniTask Summon(int playerID, CardData cardData, Vector2Int targetPosition, CardData sourceObject = null, List<CardData> sourceObjects = null, int spawnLevel = 1)
    {
        if (tokenManager.IsTokenAtGridPosition(targetPosition))
        {
            Debug.Log("해당 위치에 유닛이 존재합니다.");
            return;
        }

        Vector3 position = gridManager.GetWorldPosition(targetPosition);
        Quaternion rotation = Quaternion.Euler(90f, 0f, 0f);
        Vector3 scale = Vector3.one;

        PRS prs = new PRS(position, rotation, scale);

        // 토큰 생성 후 위에서 떨어지는 연출을 위해 높은 위치에서 시작
        Token created = await tokenFactory.CreateToken(cardData, playerID, sourceObject, sourceObjects, spawnLevel);
        created.transform.position = position + Vector3.up * 10f;
        created.transform.rotation = rotation;

        if (created.Tag == UnitTag.King)
            tokenManager.AddKingToken(playerID, created);

        tokenManager.PlaceTokenTo(created, targetPosition);

        var task = new UniTaskCompletionSource();

        // 낙하 애니메이션 완료까지 대기
        if (created.TryGetComponent<TokenMovement>(out TokenMovement movement))
        {
            movement.MoveTransform(prs, 0.5f, false, () =>
            {
                task.TrySetResult();
            });

            await task.Task;
        }
    }
}
