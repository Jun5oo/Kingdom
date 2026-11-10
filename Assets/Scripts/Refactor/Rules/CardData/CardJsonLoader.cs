using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace Refactor
{
    // 카드 JSON을 CardDefinition으로 바꾸고 검증한다. 데이터 오류는 파일 이름과 위치를 담아 InvalidDataException으로 던진다.
    public static class CardJsonLoader
    {
        // JSON의 "type" 이름 → 구체 클래스. 새 부품 클래스를 만들면 여기에 등록한다.
        private static readonly Dictionary<Type, Dictionary<string, Type>> TypesByBase = new()
        {
            [typeof(MovementRule)] = new()
            {
                ["Line"] = typeof(LineMovementRule),
                ["Knight"] = typeof(KnightMovementRule),
            },
            [typeof(Condition)] = new()
            {
                ["All"] = typeof(AllCondition),
                ["Any"] = typeof(AnyCondition),
                ["Not"] = typeof(NotCondition),
            },
            [typeof(Cost)] = new()
            {
                ["All"] = typeof(AllCost),
            },
            [typeof(Target)] = new(),
            [typeof(Operation)] = new()
            {
                ["Damage"] = typeof(DamageOperation),
            },
            [typeof(Amount)] = new()
            {
                ["Flat"] = typeof(FlatAmount),
                ["CountScaled"] = typeof(CountScaledAmount),
            },
        };

        private static readonly JsonSerializerSettings Settings = new()
        {
            // 오타 난 항목 이름이 조용히 무시되지 않게 한다.
            MissingMemberHandling = MissingMemberHandling.Error,
            // C# 프로퍼티 BaseCP는 JSON의 baseCP로 읽는다.
            ContractResolver = new DefaultContractResolver { NamingStrategy = new CamelCaseNamingStrategy() },
            Converters = { new StringEnumConverter(), new TypeNameConverter() },
        };

        // 폴더의 *.json을 모두 읽는다. 순서를 고정하려고 파일 이름으로 정렬한다.
        public static List<CardDefinition> LoadDirectory(string directory)
        {
            IEnumerable<(string fileName, string json)> files = Directory
                .GetFiles(directory, "*.json")
                .OrderBy(path => path, StringComparer.Ordinal)
                .Select(path => (Path.GetFileName(path), File.ReadAllText(path)));
            return Load(files);
        }

        public static List<CardDefinition> Load(IEnumerable<(string fileName, string json)> files)
        {
            List<CardDefinition> cards = new();
            foreach ((string fileName, string json) in files)
            {
                CardDefinition card;
                try
                {
                    card = JsonConvert.DeserializeObject<CardDefinition>(json, Settings);
                }
                catch (JsonException exception)
                {
                    throw new InvalidDataException($"{fileName}: {exception.Message}", exception);
                }

                if (card == null)
                    throw new InvalidDataException($"{fileName}: 파일이 비어 있습니다.");

                string error = Validate(card);
                if (error != null)
                    throw new InvalidDataException($"{fileName}: {error}");

                cards.Add(card);
            }

            return cards;
        }

        // JSON 형식으로는 잡히지 않는 값의 범위를 검사한다. 문제가 없으면 null.
        private static string Validate(CardDefinition card)
        {
            if (string.IsNullOrWhiteSpace(card.CardId))
                return "id가 비어 있습니다.";
            if (card.Levels.Count == 0)
                return "levels가 비어 있습니다.";

            HashSet<int> seenLevels = new();
            for (int i = 0; i < card.Levels.Count; i++)
            {
                UnitLevelData level = card.Levels[i];
                if (level == null)
                    return $"levels[{i}]가 비어 있습니다.";
                if (level.Level < 1)
                    return $"levels[{i}].level은 1 이상이어야 합니다.";
                if (!seenLevels.Add(level.Level))
                    return $"levels[{i}].level {level.Level}이 중복됩니다.";
                if (level.BaseCP < 0)
                    return $"levels[{i}].baseCP는 0 이상이어야 합니다.";
                if (level.BaseHP < 1)
                    return $"levels[{i}].baseHP는 1 이상이어야 합니다.";
                if (level.Movement == null)
                    return $"levels[{i}].movement가 비어 있습니다.";
                if (level.Attack == null || level.Attack.MaxDistance < 0)
                    return $"levels[{i}].attack.maxDistance는 0 이상이어야 합니다.";
            }

            return null;
        }

        // "type" 값으로 구체 클래스를 골라 채운다. 등록표에 없는 이름은 오류로 처리한다.
        private sealed class TypeNameConverter : JsonConverter
        {
            public override bool CanWrite => false;

            public override bool CanConvert(Type objectType) => TypesByBase.ContainsKey(objectType);

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
                JsonSerializer serializer)
            {
                if (reader.TokenType == JsonToken.Null)
                    return null;

                string path = reader.Path;
                JObject json = JObject.Load(reader);
                string typeName = (string)json["type"];
                if (typeName == null || !TypesByBase[objectType].TryGetValue(typeName, out Type type))
                    throw new JsonSerializationException($"{path}.type '{typeName}'를 알 수 없습니다.");

                // 대상 클래스에는 type 항목이 없으므로 제거해야 MissingMemberHandling에 걸리지 않는다.
                json.Remove("type");
                object result = Activator.CreateInstance(type, true);
                try
                {
                    using JsonReader objectReader = json.CreateReader();
                    serializer.Populate(objectReader, result);
                }
                catch (JsonException exception)
                {
                    throw new JsonSerializationException($"{path}: {exception.Message}", exception);
                }

                return result;
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
                => throw new NotSupportedException();
        }
    }
}
