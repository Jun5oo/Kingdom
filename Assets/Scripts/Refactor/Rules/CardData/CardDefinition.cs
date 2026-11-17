using System.Collections.Generic;
using Newtonsoft.Json;

namespace Refactor
{
    // 카드 JSON 파일 하나(와 같은 이름의 Lua 파일)에서 읽은 카드 정의.
    public sealed class CardDefinition
    {
        [JsonProperty("id", Required = Required.Always)] public string CardId { get; private set; }
        [JsonProperty("name")] public string CardName { get; private set; }
        [JsonProperty] public string Description { get; private set; }
        [JsonProperty] public Race Race { get; private set; }
        [JsonProperty] public UnitTag Tag { get; private set; }

        // 기물 등급(폰 1, 비숍·룩·나이트 2, 퀸 3). 합성이 없으므로 카드마다 고정이다.
        [JsonProperty(Required = Required.Always)] public int Level { get; private set; }
        [JsonProperty(Required = Required.Always)] public int BaseCP { get; private set; }
        [JsonProperty(Required = Required.Always)] public int BaseHP { get; private set; }
        [JsonProperty(Required = Required.Always)] public MovementRule Movement { get; private set; }
        [JsonProperty] public RangeData Attack { get; private set; } = new();

        // JSON이 아니라 같은 이름의 Lua 파일에서 채운다.
        public IReadOnlyList<EffectData> Effects { get; internal set; } = new List<EffectData>();
    }
}
