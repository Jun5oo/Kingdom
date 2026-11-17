using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MoonSharp.Interpreter;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace Refactor
{
    // 카드 JSON(수치)과 같은 이름의 Lua 파일(효과)을 CardDefinition으로 바꾸고 검증한다.
    // 데이터 오류는 파일 이름과 위치를 담아 InvalidDataException으로 던진다.
    public static class CardLoader
    {
        // JSON의 "type" 이름 → 구체 클래스. 새 부품 클래스를 만들면 여기에 등록한다.
        private static readonly Dictionary<Type, Dictionary<string, Type>> TypesByBase = new()
        {
            [typeof(MovementRule)] = new()
            {
                ["Line"] = typeof(LineMovementRule),
                ["Knight"] = typeof(KnightMovementRule),
            },
        };

        // Lua 효과 테이블에 쓸 수 있는 항목. 오타 난 항목이 조용히 무시되지 않게 한다.
        private static readonly HashSet<string> EffectKeys = new()
        {
            "type", "trigger", "usesPerTurn", "cost", "condition", "target", "targetCount", "operation",
        };

        private static readonly HashSet<string> CostKeys = new() { "abilityResource", "actionResource" };

        private static readonly JsonSerializerSettings Settings = new()
        {
            // 오타 난 항목 이름이 조용히 무시되지 않게 한다.
            MissingMemberHandling = MissingMemberHandling.Error,
            // C# 프로퍼티 BaseCP는 JSON의 baseCP로 읽는다.
            ContractResolver = new DefaultContractResolver { NamingStrategy = new CamelCaseNamingStrategy() },
            Converters = { new StringEnumConverter(), new TypeNameConverter() },
        };

        // 폴더의 *.json과 *.lua를 모두 읽는다. 순서를 고정하려고 파일 이름으로 정렬한다.
        public static List<CardDefinition> LoadDirectory(string directory)
        {
            IEnumerable<(string fileName, string text)> files = Directory
                .GetFiles(directory, "*.json")
                .Concat(Directory.GetFiles(directory, "*.lua"))
                .OrderBy(path => path, StringComparer.Ordinal)
                .Select(path => (Path.GetFileName(path), File.ReadAllText(path)));
            return Load(files);
        }

        // 확장자가 .lua인 파일은 같은 이름의 JSON 카드의 효과로, 나머지는 카드 JSON으로 읽는다.
        public static List<CardDefinition> Load(IEnumerable<(string fileName, string text)> files)
        {
            List<CardDefinition> cards = new();
            List<string> fileNames = new();
            List<(string fileName, string code)> scripts = new();
            foreach ((string fileName, string text) in files)
            {
                if (Path.GetExtension(fileName) == ".lua")
                {
                    scripts.Add((fileName, text));
                    continue;
                }

                CardDefinition card;
                try
                {
                    card = JsonConvert.DeserializeObject<CardDefinition>(text, Settings);
                }
                catch (JsonException exception)
                {
                    throw new InvalidDataException($"{fileName}: {exception.Message}", exception);
                }

                if (card == null)
                    throw new InvalidDataException($"{fileName}: 파일이 비어 있습니다.");

                cards.Add(card);
                fileNames.Add(fileName);
            }

            for (int i = 0; i < cards.Count; i++)
            {
                string error = Validate(cards[i]);
                if (error != null)
                    throw new InvalidDataException($"{fileNames[i]}: {error}");
            }

            foreach ((string fileName, string code) in scripts)
            {
                string baseName = Path.GetFileNameWithoutExtension(fileName);
                int index = fileNames.FindIndex(name => Path.GetFileNameWithoutExtension(name) == baseName);
                if (index < 0)
                    throw new InvalidDataException($"{fileName}: 같은 이름의 카드 JSON 파일이 없습니다.");

                LoadScript(fileName, code, cards[index]);
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

        // 스크립트는 { [등급] = { 효과, ... } } 테이블을 돌려준다. 실행해서 받은 함수는 EffectData가 보관한다.
        private static void LoadScript(string fileName, string code, CardDefinition card)
        {
            // 파일, OS 접근 같은 모듈을 빼서 카드 코드가 엔진 밖을 건드리지 못하게 한다.
            Script script = new(CoreModules.Preset_HardSandbox);
            DynValue result;
            try
            {
                result = script.DoString(code, null, fileName);
            }
            catch (InterpreterException exception)
            {
                throw new InvalidDataException(exception.DecoratedMessage ?? $"{fileName}: {exception.Message}", exception);
            }

            if (result.Type != DataType.Table)
                throw new InvalidDataException($"{fileName}: 등급별 효과 테이블을 return 해야 합니다.");

            foreach (TablePair pair in result.Table.Pairs)
            {
                if (!TryGetInt(pair.Key, out int level) || !card.TryGetLevel(level, out UnitLevelData data))
                    throw new InvalidDataException($"{fileName}: [{pair.Key.ToPrintString()}]에 해당하는 등급이 JSON에 없습니다.");
                if (pair.Value.Type != DataType.Table)
                    throw new InvalidDataException($"{fileName}: [{level}]은 효과 목록이어야 합니다.");

                Table list = pair.Value.Table;
                List<EffectData> effects = new();
                for (int i = 1; i <= list.Length; i++)
                {
                    string at = $"{fileName}: [{level}][{i}]";
                    if (list.Get(i).Type != DataType.Table)
                        throw new InvalidDataException($"{at}는 효과 테이블이어야 합니다.");

                    effects.Add(ParseEffect(list.Get(i).Table, at));
                }

                data.Effects = effects;
            }
        }

        private static EffectData ParseEffect(Table table, string at)
        {
            foreach (DynValue key in table.Keys)
            {
                if (key.Type != DataType.String || !EffectKeys.Contains(key.String))
                    throw new InvalidDataException($"{at}: 알 수 없는 항목 {key.ToPrintString()}");
            }

            EffectType type = ParseEnum<EffectType>(table.Get("type"), "type", at, required: true);
            EventTrigger trigger = ParseEnum<EventTrigger>(table.Get("trigger"), "trigger", at, required: false);
            if (type == EffectType.Trigger && trigger == EventTrigger.None)
                throw new InvalidDataException($"{at}: Trigger 효과에는 trigger가 필요합니다.");

            int usesPerTurn = ParseInt(table.Get("usesPerTurn"), "usesPerTurn", at, defaultValue: 0, min: 0);
            int targetCount = ParseInt(table.Get("targetCount"), "targetCount", at, defaultValue: 1, min: 1);

            int abilityCost = 0;
            int actionCost = 0;
            DynValue cost = table.Get("cost");
            if (!cost.IsNil())
            {
                if (cost.Type != DataType.Table)
                    throw new InvalidDataException($"{at}: cost는 테이블이어야 합니다.");
                foreach (DynValue key in cost.Table.Keys)
                {
                    if (key.Type != DataType.String || !CostKeys.Contains(key.String))
                        throw new InvalidDataException($"{at}: cost에 알 수 없는 항목 {key.ToPrintString()}");
                }

                abilityCost = ParseInt(cost.Table.Get("abilityResource"), "cost.abilityResource", at, defaultValue: 0, min: 0);
                actionCost = ParseInt(cost.Table.Get("actionResource"), "cost.actionResource", at, defaultValue: 0, min: 0);
            }

            Closure operation = ParseFunction(table.Get("operation"), "operation", at)
                ?? throw new InvalidDataException($"{at}: operation 함수가 필요합니다.");

            return new EffectData(type, trigger, usesPerTurn, abilityCost, actionCost, targetCount,
                ParseFunction(table.Get("condition"), "condition", at),
                ParseFunction(table.Get("target"), "target", at),
                operation);
        }

        private static T ParseEnum<T>(DynValue value, string name, string at, bool required) where T : struct, Enum
        {
            if (value.IsNil() && !required)
                return default;
            if (value.Type != DataType.String || !Enum.TryParse(value.String, out T result))
                throw new InvalidDataException($"{at}: {name}에 알 수 없는 값 {value.ToPrintString()}");
            return result;
        }

        private static int ParseInt(DynValue value, string name, string at, int defaultValue, int min)
        {
            if (value.IsNil())
                return defaultValue;
            if (!TryGetInt(value, out int result) || result < min)
                throw new InvalidDataException($"{at}: {name}은 {min} 이상의 정수여야 합니다.");
            return result;
        }

        // 함수가 없으면 null.
        private static Closure ParseFunction(DynValue value, string name, string at)
        {
            if (value.IsNil())
                return null;
            if (value.Type != DataType.Function)
                throw new InvalidDataException($"{at}: {name}은 함수여야 합니다.");
            return value.Function;
        }

        private static bool TryGetInt(DynValue value, out int result)
        {
            result = 0;
            if (value.Type != DataType.Number || value.Number != Math.Floor(value.Number))
                return false;
            result = (int)value.Number;
            return true;
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
