using System.Collections.Generic;
using UnityEngine;

namespace Refactor
{
    [CreateAssetMenu(menuName = "Kingdom/Refactor/Unit Card")]
    public sealed class UnitCardData : CardData
    {
        [SerializeField] private Race race;
        [SerializeField] private UnitTag tag;

        [SerializeField] private List<UnitLevelData> levels = new();

        public Race Race => race;
        public UnitTag Tag => tag;
        public IReadOnlyList<UnitLevelData> Levels => levels;

        public bool TryGetLevel(int level, out UnitLevelData result)
        {
            foreach (UnitLevelData data in levels)
            {
                if (data != null && data.Level == level)
                {
                    result = data;
                    return true;
                }
            }

            result = null;
            return false;
        }
    }
}
