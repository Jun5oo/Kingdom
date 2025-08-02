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

    public async UniTask Summon(int playerID, UnitCardData unitCardData, Vector2Int position)
    {
        // 1. 생성 
        Token created = await tokenFactory.CreateToken(unitCardData, playerID);

        // 2. 소환 애니메이션 
        Vector3 targetPosition = gridManager.GetWorldPosition(position);

        // 3. 생성된 토큰 배치 
        tokenManager.PlaceTokenTo(created, position); 
    }
}
