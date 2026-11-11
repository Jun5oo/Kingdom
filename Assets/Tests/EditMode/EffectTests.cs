using System.IO;
using System.Linq;
using NUnit.Framework;
using Refactor;

namespace Kingdom.Tests
{
    public sealed class EffectTests
    {
        // 카드 ① 사망 트리거: 내가 상대 영웅을 죽이면 자원 +1, 아군 영웅이 죽으면 그 칸에 무덤 소환.
        private const string WatcherEffects = @"
            { ""type"": ""Trigger"", ""trigger"": ""OnUnitDead"",
              ""condition"": { ""type"": ""DeadUnit"", ""owner"": ""Opponent"", ""tag"": ""Hero"", ""killer"": ""Self"" },
              ""operations"": [ { ""type"": ""ChangeAbilityResource"", ""amount"": 1 } ] },
            { ""type"": ""Trigger"", ""trigger"": ""OnUnitDead"",
              ""condition"": { ""type"": ""DeadUnit"", ""owner"": ""Self"", ""tag"": ""Hero"" },
              ""operations"": [ { ""type"": ""Summon"", ""cardId"": ""grave"" } ] }";

        // 카드 ② 자기희생: 턴당 1회 스스로를 릴리스하고 자원 +1.
        private const string MartyrEffects = @"
            { ""type"": ""Ignition"", ""usesPerTurn"": 1,
              ""operations"": [ { ""type"": ""Release"" }, { ""type"": ""ChangeAbilityResource"", ""amount"": 1 } ] }";

        private const string DrummerEffects = @"
            { ""type"": ""Ignition"", ""usesPerTurn"": 1,
              ""operations"": [ { ""type"": ""ChangeAbilityResource"", ""amount"": 1 } ] }";

        private const string AvengerEffects = @"
            { ""type"": ""Trigger"", ""trigger"": ""OnUnitDead"",
              ""condition"": { ""type"": ""DeadUnit"", ""owner"": ""Opponent"", ""tag"": ""Hero"" },
              ""operations"": [ { ""type"": ""Release"" } ] }";

        private const string GravediggerEffects = @"
            { ""type"": ""Trigger"", ""trigger"": ""OnUnitDead"",
              ""operations"": [ { ""type"": ""Summon"", ""cardId"": ""grave"" } ] }";

        private GameEngine engine;

        // 상하좌우 1칸을 이동·공격할 수 있는 CP 10 카드.
        private static string UnitJson(string id, string tag, string effects = "") => $@"{{
            ""id"": ""{id}"", ""name"": ""{id}"", ""tag"": ""{tag}"",
            ""levels"": [ {{
                ""level"": 1, ""baseCP"": 10, ""baseHP"": 10,
                ""movement"": {{ ""type"": ""Line"", ""directions"": ""Orthogonal"", ""maxDistance"": 1 }},
                ""attack"": {{ ""directions"": ""Orthogonal"", ""maxDistance"": 1 }},
                ""effects"": [ {effects} ]
            }} ]
        }}";

        private void StartGame(params UnitPlacement[] placements)
        {
            CardDatabase database = new(CardJsonLoader.Load(new[]
            {
                ("fighter.json", UnitJson("fighter", "Normal")),
                ("hero.json", UnitJson("hero", "Hero")),
                ("grave.json", UnitJson("grave", "Graveyard")),
                ("watcher.json", UnitJson("watcher", "Normal", WatcherEffects)),
                ("martyr.json", UnitJson("martyr", "Normal", MartyrEffects)),
                ("drummer.json", UnitJson("drummer", "Normal", DrummerEffects)),
                ("avenger.json", UnitJson("avenger", "Normal", AvengerEffects)),
                ("gravedigger.json", UnitJson("gravedigger", "Normal", GravediggerEffects)),
            }));
            engine = new GameEngine(new GameState(new PlayerState(1), new PlayerState(2)), database);
            Assert.IsTrue(engine.StartGame(placements, out string reason), reason);
        }

        private static UnitPlacement Place(int ownerId, string cardId, int x, int y)
            => new(ownerId, cardId, 1, new GridPos(x, y));

        private void SubmitOk(int playerId, GameCommand command)
        {
            SubmitResult result = engine.Submit(playerId, command);
            Assert.IsTrue(result.Succeeded, result.Reason);
        }

        [Test]
        public void KillingOpponentHeroGivesAbilityResource()
        {
            StartGame(Place(1, "watcher", 0, 0), Place(1, "fighter", 2, 0), Place(2, "hero", 2, 1));

            SubmitResult result = engine.Submit(1, new AttackCommand(2, 3));

            Assert.IsTrue(result.Succeeded, result.Reason);
            Assert.AreEqual(1, engine.State.Players[1].AbilityResource);
            Assert.AreEqual(0, engine.State.Players[2].AbilityResource);
            Assert.AreEqual(1, ((UnitDied)result.Events.Single(e => e is UnitDied)).KillerPlayerId);
            Assert.IsFalse(result.Events.OfType<UnitSummoned>().Any());
        }

        [Test]
        public void AllyHeroDeathSummonsGraveAtThatCell()
        {
            StartGame(Place(1, "watcher", 0, 0), Place(1, "hero", 2, 1), Place(2, "fighter", 2, 0));
            SubmitOk(1, new EndTurnCommand());

            SubmitResult result = engine.Submit(2, new AttackCommand(3, 2));

            Assert.IsTrue(result.Succeeded, result.Reason);
            UnitSummoned summoned = result.Events.OfType<UnitSummoned>().Single();
            Assert.AreEqual(4, summoned.UnitId);
            UnitState grave = engine.State.Units[engine.State.Board.GetUnitIdAt(2, 1).Value];
            Assert.AreEqual("grave", grave.CardDefinitionId);
            Assert.AreEqual(1, grave.OwnerId);
            Assert.AreEqual(0, engine.State.Players[1].AbilityResource);
        }

        [Test]
        public void SelfReleaseRemovesUnitAndGivesAbilityResource()
        {
            StartGame(Place(1, "martyr", 0, 0), Place(2, "fighter", 4, 6));
            Assert.IsTrue(engine.GetAvailableCommands(1).OfType<ActivateEffectCommand>().Any(c => c.UnitId == 1));

            SubmitResult result = engine.Submit(1, new ActivateEffectCommand(1, 0));

            Assert.IsTrue(result.Succeeded, result.Reason);
            UnitDied died = (UnitDied)result.Events[0];
            Assert.AreEqual(DeathCause.Release, died.Cause);
            Assert.AreEqual(new GridPos(0, 0), died.Position);
            Assert.IsInstanceOf<AbilityResourceChanged>(result.Events[1]);
            Assert.IsFalse(engine.State.Units.ContainsKey(1));
            Assert.AreEqual(1, engine.State.Players[1].AbilityResource);
            Assert.AreEqual(GameEngine.ActionsPerTurn, engine.State.Players[1].ActionResource);
        }

        [Test]
        public void IgnitionEffectIsLimitedPerTurn()
        {
            StartGame(Place(1, "drummer", 0, 0), Place(2, "fighter", 4, 6));
            SubmitOk(1, new ActivateEffectCommand(1, 0));

            SubmitResult second = engine.Submit(1, new ActivateEffectCommand(1, 0));

            Assert.IsFalse(second.Succeeded);
            Assert.IsEmpty(second.Events);
            Assert.AreEqual(1, engine.State.Players[1].AbilityResource);
            Assert.IsFalse(engine.GetAvailableCommands(1).OfType<ActivateEffectCommand>().Any());

            SubmitOk(1, new EndTurnCommand());
            SubmitOk(2, new EndTurnCommand());
            SubmitOk(1, new ActivateEffectCommand(1, 0));
            Assert.AreEqual(2, engine.State.Players[1].AbilityResource);
        }

        [Test]
        public void TriggerFromResolutionIsQueuedBehindPendingTriggers()
        {
            // 영웅 사망 → [복수자 릴리스, 무덤꾼(영웅)] 순서로 큐. 복수자 사망으로 생긴 무덤꾼 트리거는 뒤에 붙는다.
            StartGame(Place(1, "avenger", 0, 0), Place(1, "gravedigger", 6, 0),
                Place(1, "fighter", 2, 0), Place(2, "hero", 2, 1));

            SubmitResult result = engine.Submit(1, new AttackCommand(3, 4));

            Assert.IsTrue(result.Succeeded, result.Reason);
            GridPos[] summonedAt = result.Events.OfType<UnitSummoned>().Select(e => e.Position).ToArray();
            CollectionAssert.AreEqual(new[] { new GridPos(2, 1), new GridPos(0, 0) }, summonedAt);
            Assert.AreEqual(DeathCause.Release, result.Events.OfType<UnitDied>().Last().Cause);
        }

        [Test]
        public void SummonOfUnknownCardFailsToLoad()
        {
            InvalidDataException exception = Assert.Throws<InvalidDataException>(
                () => CardJsonLoader.Load(new[] { ("digger.json", UnitJson("digger", "Normal", GravediggerEffects)) }));

            StringAssert.Contains("digger.json", exception.Message);
            StringAssert.Contains("grave", exception.Message);
        }
    }
}
