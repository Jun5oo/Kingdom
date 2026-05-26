using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어별 덱(CardData 큐)과 왕 카드를 관리하는 매니저.
/// 현재는 인스펙터에서 직접 지정한 고정덱을 사용하며, Race에 따라 Undead/Celestial 덱을 배분한다.
/// GetCardData()는 큐에서 순서대로 꺼내므로 호출할 때마다 덱이 소비된다.
/// </summary>
public class DeckManager : MonoBehaviour
{
    PlayerManager playerManager;

    // 임시 고정덱 — 추후 동적 덱 빌딩으로 교체 예정
    [SerializeField] List<CardData> undeadDeck;
    [SerializeField] List<CardData> celestialDeck;

    [SerializeField] CardData undead;    // Undead 왕 카드 데이터
    [SerializeField] CardData celestial; // Celestial 왕 카드 데이터

    private Dictionary<int, Queue<CardData>> playerDeck; // 플레이어 ID → 드로우 큐
    private Dictionary<int, CardData> playerKing;        // 플레이어 ID → 왕 카드 데이터

    /// <summary> 양쪽 플레이어의 덱과 왕 카드를 종족에 맞게 초기화한다. </summary>
    public void Init()
    {
        playerManager = ServiceLocator.Get<PlayerManager>();

        playerDeck = new Dictionary<int, Queue<CardData>>();
        playerKing = new Dictionary<int, CardData>();

        playerDeck[playerManager.Local.PlayerID] = CreateDeck(playerManager.Local.Race);
        playerDeck[playerManager.Remote.PlayerID] = CreateDeck(playerManager.Remote.Race);

        playerKing[playerManager.Local.PlayerID] = CreateKing(playerManager.Local.Race);
        playerKing[playerManager.Remote.PlayerID] = CreateKing(playerManager.Remote.Race);
    }

    /// <summary> 종족에 맞는 고정 덱 리스트를 Queue로 변환하여 반환한다. </summary>
    public Queue<CardData> CreateDeck(Race race)
    {
        switch(race)
        {
            case Race.Undead:
                return new Queue<CardData>(undeadDeck);
            case Race.Celestial:
                return new Queue<CardData>(celestialDeck);
        }

        Debug.LogError("Undefined race");
        return null;
    }

    /// <summary> 종족에 맞는 왕 CardData를 반환한다. </summary>
    public CardData CreateKing(Race race)
    {
        switch (race)
        {
            case Race.Undead:
                return undead;
            case Race.Celestial:
                return celestial;
        }

        Debug.LogError("Undefined race");
        return null;
    }

    /// <summary> 덱에서 다음 CardData를 꺼내 반환한다. 덱이 비어 있으면 null을 반환한다. </summary>
    public CardData GetCardData(int playerID)
    {
        if(IsDeckEmpty(playerID))
            return null;

        CardData cardData = playerDeck[playerID].Dequeue();
        return cardData;
    }

    /// <summary> 해당 플레이어의 왕 CardData를 반환한다. </summary>
    public CardData GetKingCardData(int playerID)
    {
        return playerKing[playerID];
    }

    /// <summary> 해당 플레이어의 덱이 비어 있는지 여부를 반환한다. </summary>
    public bool IsDeckEmpty(int playerID)
    {
        if (!playerDeck.ContainsKey(playerID))
        {
            Debug.LogError("잘못된 플레이어ID 입니다.");
            return false;
        }

        return playerDeck[playerID].Count <= 0;
    }
}
