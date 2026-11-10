using NUnit.Framework;
using Refactor;

namespace Kingdom.Tests
{
    public sealed class GameEngineTests
    {
        private GameEngine engine;

        [SetUp]
        public void StartGameWithTwoPawns()
        {
            CardDatabase database = new(CardJsonLoader.Load(new[] { ("pawn.json", CardJsonLoaderTests.PawnJson) }));
            engine = new GameEngine(new GameState(new PlayerState(1), new PlayerState(2)), database);

            UnitPlacement[] placements =
            {
                new(1, "pawn", 1, new GridPos(2, 0)),
                new(2, "pawn", 1, new GridPos(4, 6)),
            };
            Assert.IsTrue(engine.StartGame(placements, out string reason), reason);
        }

        [Test]
        public void MoveOneStepSucceeds()
        {
            Assert.IsTrue(engine.MoveUnit(1, 1, 2, 1, out string reason), reason);

            Assert.IsTrue(engine.State.Board.TryGetPosition(1, out int x, out int y));
            Assert.AreEqual((2, 1), (x, y));
        }

        [Test]
        public void MoveBeyondMaxDistanceFailsWithoutChangingState()
        {
            Assert.IsFalse(engine.MoveUnit(1, 1, 2, 2, out _));

            Assert.IsTrue(engine.State.Board.TryGetPosition(1, out int x, out int y));
            Assert.AreEqual((2, 0), (x, y));
        }
    }
}
