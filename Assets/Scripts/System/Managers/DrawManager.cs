using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 덱에서 카드 데이터를 뽑아 Card 오브젝트로 생성하는 매니저.
/// DeckManager에서 CardData를 가져오고 CardFactory로 실제 오브젝트를 비동기 생성한다.
/// </summary>
public class DrawManager
{
    DeckManager deckManager;
    CardFactory cardFactory;

    public void Init()
    {
        deckManager = ServiceLocator.Get<DeckManager>();
        cardFactory = ServiceLocator.Get<CardFactory>();
    }

    /// <summary> 덱에서 일반 카드를 한 장 드로우하여 Card 오브젝트를 반환한다. 덱이 비어 있으면 null을 반환한다. </summary>
    public async UniTask<Card> Draw(int playerID)
    {
        CardData cardData = deckManager.GetCardData(playerID);

        if(cardData == null)
        {
            Debug.Log("덱에서부터 카드를 가져올 수 없습니다.");
            return null;
        }

        Card card = await cardFactory.CreateCardAsync(cardData, playerID);
        return card;
    }

    /// <summary> 왕 카드를 드로우하여 Card 오브젝트를 반환한다. 왕 카드가 없으면 null을 반환한다. </summary>
    public async UniTask<Card> DrawKing(int playerID)
    {
        CardData cardData = deckManager.GetKingCardData(playerID);

        if(cardData == null)
        {
            Debug.Log("덱에서부터 왕 카드를 가져올 수 없습니다.");
            return null;
        }

        Card card = await cardFactory.CreateCardAsync(cardData, playerID);
        return card;
    }
}
