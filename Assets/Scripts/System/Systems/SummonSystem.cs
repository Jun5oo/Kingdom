using Cysharp.Threading.Tasks;
using UnityEngine;

public class SummonSystem
{
    // SummonAction, SummonGraveyard 등 Summon 중복되는 부분이 많아서 SummonSystem 작성 중. 

    TokenManager tokenManager;
    TokenFactory tokenFactory;
    GridManager gridManager; 

    public void Init()
    {
        tokenManager = ServiceLocator.Get<TokenManager>(); 
        tokenFactory = ServiceLocator.Get<TokenFactory>();
        gridManager = ServiceLocator.Get<GridManager>();
    }

    public async UniTask Summon(int playerID, UnitCardData unitCardData, Vector2Int targetPosition)
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

        // 1. 생성 
        Token created = await tokenFactory.CreateToken(unitCardData, playerID);
        created.transform.position = position + Vector3.up * 10f;
        created.transform.rotation = rotation;

        if(created.IsKing)
            tokenManager.AddKingToken(playerID, created);

        tokenManager.PlaceTokenTo(created, targetPosition);

        var task = new UniTaskCompletionSource();

        // 2. 애니메이션  
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
