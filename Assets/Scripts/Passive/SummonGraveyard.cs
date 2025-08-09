using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

public class SummonGraveyard : IPassive
{
    // 언데드왕의 패시브, 아군이 처치되었을 때 발동
    // 전체적으로 수정이 필요함 
    BaseObject passiveOwner; 

    TokenManager tokenManager;
    GridManager gridManager; 
    DamageManager damageManager; 

    public SummonGraveyard(BaseObject owner) 
    {
        tokenManager = ServiceLocator.Get<TokenManager>(); 
        gridManager = ServiceLocator.Get<GridManager>();    
        damageManager = ServiceLocator.Get<DamageManager>();

        this.passiveOwner = owner; 
    }

    public void Activate()
    {
        Debug.Log($"{this} 연결되었습니다."); 
        damageManager.OnPlayerUnitDead += Summon; 
    }

    public void Deactivate()
    {
        damageManager.OnPlayerUnitDead -= Summon; 
    }

    public void Summon(int playerID, Token deadToken)
    {
        if (playerID != passiveOwner.OwnerID)
            return; 

        Vector2Int position = tokenManager.GetGridPositionOfToken(deadToken);
        Vector3 worldPos = gridManager.GetWorldPosition(position);

        // Temp 
        CardDatabase database = ServiceLocator.Get<CardDatabase>();

        var unitData = deadToken.UnitData;

        UnitCardData graveyard = database.GetData("undead_graveyard");

        EventQueue eventQueue = ServiceLocator.Get<EventQueue>();
        TokenFactory tokenFactory = ServiceLocator.Get<TokenFactory>();

        eventQueue.Enqueue(async () =>
        {
            await UniTask.Delay(100); 
            if (tokenManager.IsTokenAtGridPosition(position))
            {
                Debug.Log("Graveyard가 소환될 자리에 다른 유닛이 존재합니다.");
                return;
            }

            Vector3 targetPos = worldPos;
            Vector3 eulerAngles = new Vector3(90f, 0f, 0f);
            Quaternion quaternion = Quaternion.Euler(eulerAngles);
            Vector3 scale = Vector3.one;

            PRS prs = new PRS(targetPos, quaternion, scale);

            Token _graveyard = await tokenFactory.CreateToken(graveyard, playerID);
            TokenMovement tokenMovement = _graveyard.GetComponent<TokenMovement>();
            tokenMovement.MoveTransform(prs, 0, false); 

            Debug.Log("Token Summon Complete");

            _graveyard.transform.position = targetPos;
            _graveyard.transform.rotation = quaternion;

            tokenManager.PlaceTokenTo(_graveyard, position);
        }); 
    }
}
