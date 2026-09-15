using System.Collections.Generic;
using UnityEngine;

namespace Refactor
{
    public abstract class TestCardData : ScriptableObject
    {
        [SerializeField] string cardId;
        [SerializeField] string cardName;
        [TextArea][SerializeField] string cardDescription;

        [SerializeField] List<EffectData> effects = new();

        public string CardId { get { return cardId; } }
        public string CardName { get { return cardName; } }
        public string CardDescription { get { return cardDescription; } }
        public IReadOnlyList<EffectData> Effects { get { return effects; } }
    }
}
