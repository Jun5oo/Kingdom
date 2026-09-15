using UnityEngine;

namespace Refactor
{
    public abstract class CardData : ScriptableObject
    {
        [SerializeField] private string cardId;
        [SerializeField] private string cardName;

        [TextArea]
        [SerializeField] private string description;

        [SerializeField] private Sprite artwork;

        public string CardId => cardId;
        public string CardName => cardName;
        public string Description => description;
        public Sprite Artwork => artwork;
    }
}
