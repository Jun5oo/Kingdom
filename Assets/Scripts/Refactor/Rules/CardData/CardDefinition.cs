using System.Collections.Generic;
using Newtonsoft.Json;

namespace Refactor
{
    // 카드 JSON 파일 하나에서 읽은 카드 정의.
    public sealed class CardDefinition
    {
        [JsonProperty("id", Required = Required.Always)] public string CardId { get; private set; }
        [JsonProperty("name")] public string CardName { get; private set; }
        [JsonProperty] public string Description { get; private set; }
        [JsonProperty] public Race Race { get; private set; }
        [JsonProperty] public UnitTag Tag { get; private set; }

        [JsonProperty(Required = Required.Always)]
        public IReadOnlyList<UnitLevelData> Levels { get; private set; } = new List<UnitLevelData>();

        public bool TryGetLevel(int level, out UnitLevelData result)
        {
            foreach (UnitLevelData data in Levels)
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
