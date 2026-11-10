using System.Collections.Generic;
using Newtonsoft.Json;

namespace Refactor
{
    // 카드 JSON 안의 등급별 정의. 런타임 상태와 분리한다.
    public sealed class UnitLevelData
    {
        [JsonProperty(Required = Required.Always)] public int Level { get; private set; }
        [JsonProperty(Required = Required.Always)] public int BaseCP { get; private set; }
        [JsonProperty(Required = Required.Always)] public int BaseHP { get; private set; }
        [JsonProperty(Required = Required.Always)] public MovementRule Movement { get; private set; }
        [JsonProperty] public RangeData Attack { get; private set; } = new();

        // 해당 등급의 전체 효과 목록. 이전 등급에서 자동으로 상속하지 않는다.
        [JsonProperty] public IReadOnlyList<EffectData> Effects { get; private set; } = new List<EffectData>();
    }
}
