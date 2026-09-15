using UnityEngine;

namespace Refactor
{
    [CreateAssetMenu(menuName = "Refactor/CardData/UnitCardData")]
    public class TestUnitCardData : TestCardData
    {
        [SerializeField] Race race;
        [SerializeField] UnitTag tag;
    }
}
