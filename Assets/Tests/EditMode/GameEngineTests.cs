using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Refactor;

namespace Kingdom.Tests
{
    public sealed class GameEngineTests
    {
        private GameEngine engine;

        // 상하좌우 1칸을 공격할 수 있는 테스트 카드를 만든다.
        private static string FighterJson(string id, int hp) => CardJsonLoaderTests.PawnJson
            .Replace(@"""id"": ""pawn""", $@"""id"": ""{id}""")
            .Replace(@"""baseHP"": 10", $@"""baseHP"": {hp}")
            .Replace(@"""movement""", @"""attack"": { ""directions"": ""Orthogonal"", ""maxDistance"": 1 }, ""movement""");

        private void StartGame(params UnitPlacement[] placements)
        {
            CardDatabase database = new(CardJsonLoader.Load(new[]
            {
                ("pawn.json", CardJsonLoaderTests.PawnJson),
                ("fighter.json", FighterJson("fighter", 10)),
                ("tank.json", FighterJson("tank", 30)),
            }));
            engine = new GameEngine(new GameState(new PlayerState(1), new PlayerState(2)), database);
            Assert.IsTrue(engine.StartGame(placements, out string reason), reason);
        }

        private void StartWithTwoPawns() => StartGame(
            new UnitPlacement(1, "pawn", 1, new GridPos(2, 0)),
            new UnitPlacement(2, "pawn", 1, new GridPos(4, 6)));

        private void StartAdjacent(string defenderCardId) => StartGame(
            new UnitPlacement(1, "fighter", 1, new GridPos(2, 0)),
            new UnitPlacement(2, defenderCardId, 1, new GridPos(2, 1)));

        [Test]
        public void MoveOneStepSucceedsAndReportsEvents()
        {
            StartWithTwoPawns();

            SubmitResult result = engine.Submit(1, new MoveCommand(1, new GridPos(2, 1)));

            Assert.IsTrue(result.Succeeded, result.Reason);
            Assert.IsTrue(engine.State.Board.TryGetPosition(1, out int x, out int y));
            Assert.AreEqual((2, 1), (x, y));
            Assert.IsInstanceOf<UnitMoved>(result.Events[0]);
            Assert.AreEqual(1, ((ActionResourceChanged)result.Events[1]).Remaining);
        }

        [Test]
        public void MoveBeyondMaxDistanceFailsWithoutChangingState()
        {
            StartWithTwoPawns();

            SubmitResult result = engine.Submit(1, new MoveCommand(1, new GridPos(2, 2)));

            Assert.IsFalse(result.Succeeded);
            Assert.IsNotEmpty(result.Reason);
            Assert.IsEmpty(result.Events);
            Assert.IsTrue(engine.State.Board.TryGetPosition(1, out int x, out int y));
            Assert.AreEqual((2, 0), (x, y));
            Assert.AreEqual(GameEngine.ActionsPerTurn, engine.State.Players[1].ActionResource);
        }

        [Test]
        public void NonLethalAttackKeepsDefender()
        {
            StartAdjacent("tank");

            SubmitResult result = engine.Submit(1, new AttackCommand(1, 2));

            Assert.IsTrue(result.Succeeded, result.Reason);
            Assert.AreEqual(20, engine.State.Units[2].CurrentHP);
            Assert.AreEqual(20, ((UnitDamaged)result.Events[0]).RemainingHP);
            Assert.IsFalse(result.Events.OfType<UnitDied>().Any());
        }

        [Test]
        public void LethalAttackRemovesDefenderAfterDamageAndCost()
        {
            StartAdjacent("fighter");

            SubmitResult result = engine.Submit(1, new AttackCommand(1, 2));

            Assert.IsTrue(result.Succeeded, result.Reason);
            Assert.AreEqual(3, result.Events.Count);
            Assert.IsInstanceOf<UnitDamaged>(result.Events[0]);
            Assert.IsInstanceOf<ActionResourceChanged>(result.Events[1]);
            Assert.AreEqual(DeathCause.Damage, ((UnitDied)result.Events[2]).Cause);
            Assert.IsFalse(engine.State.Units.ContainsKey(2));
            Assert.IsNull(engine.State.Board.GetUnitIdAt(2, 1));
        }

        [Test]
        public void AttackOnOpponentTurnFailsWithoutChangingState()
        {
            StartAdjacent("tank");

            SubmitResult result = engine.Submit(2, new AttackCommand(2, 1));

            Assert.IsFalse(result.Succeeded);
            Assert.AreEqual(10, engine.State.Units[1].CurrentHP);
        }

        [Test]
        public void EndTurnPassesTurnAndGrantsActions()
        {
            StartWithTwoPawns();

            SubmitResult result = engine.Submit(1, new EndTurnCommand());

            Assert.IsTrue(result.Succeeded, result.Reason);
            Assert.AreEqual(2, engine.State.CurrentPlayerId);
            Assert.AreEqual(2, engine.State.TurnNumber);
            Assert.AreEqual(GameEngine.ActionsPerTurn, engine.State.Players[2].ActionResource);
            Assert.AreEqual(2, result.Events.OfType<TurnStarted>().Single().PlayerId);
        }

        [Test]
        public void EndTurnOnOpponentTurnFails()
        {
            StartWithTwoPawns();

            Assert.IsFalse(engine.Submit(2, new EndTurnCommand()).Succeeded);
            Assert.AreEqual(1, engine.State.CurrentPlayerId);
        }

        [Test]
        public void AvailableCommandsListCurrentPlayerActionsOnly()
        {
            StartAdjacent("fighter");

            IReadOnlyList<GameCommand> commands = engine.GetAvailableCommands(1);

            Assert.IsTrue(commands.OfType<EndTurnCommand>().Any());
            Assert.IsTrue(commands.OfType<AttackCommand>().Any(c => c.AttackerId == 1 && c.DefenderId == 2));
            Assert.IsTrue(commands.OfType<MoveCommand>().Any(c => c.UnitId == 1 && c.Target.X == 1 && c.Target.Y == 0));
            Assert.IsEmpty(engine.GetAvailableCommands(2));
        }
    }
}
