using System;
using System.Collections.Generic;

namespace Refactor
{
    // 준비된 카드 에셋을 ID로 조회한다. 에셋 로딩과 유닛 생성은 담당하지 않는다.
    public sealed class CardDatabase
    {
        private readonly Dictionary<string, UnitCardData> cards = new();

        public CardDatabase(IEnumerable<UnitCardData> cardList)
        {
            if (cardList == null)
                throw new ArgumentNullException(nameof(cardList));

            foreach (UnitCardData card in cardList)
            {
                if (card == null)
                    throw new ArgumentException("카드 목록에 빈 항목이 있습니다.", nameof(cardList));

                if (string.IsNullOrWhiteSpace(card.CardId))
                    throw new ArgumentException("카드 ID가 비어 있습니다.", nameof(cardList));

                if (cards.ContainsKey(card.CardId))
                    throw new ArgumentException(
                        $"중복된 카드 ID입니다: {card.CardId}", nameof(cardList));

                cards.Add(card.CardId, card);
            }
        }

        public bool TryGetCard(string cardId, out UnitCardData card)
        {
            if (string.IsNullOrWhiteSpace(cardId))
            {
                card = null;
                return false;
            }

            return cards.TryGetValue(cardId, out card);
        }
    }
}
