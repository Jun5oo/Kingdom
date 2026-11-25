using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Refactor;

namespace Kingdom.Tests
{
    public sealed class EffectTests
    {
        // 카드 ① 사망 트리거: 내가 상대 영웅을 죽이면 자원 +1, 아군 영웅이 죽으면 그 칸에 무덤 소환.
        private const string WatcherLua = @"
            return {
              { type = 'Trigger', trigger = 'OnUnitDead',
                condition = function(ctx)
                  local dead = ctx:deadUnit()
                  return dead.ownerId ~= ctx.ownerId and ctx:tagOf(dead.cardId) == 'Hero' and dead.killerId == ctx.ownerId
                end,
                operation = function(ctx) ctx:changeAbilityResource(1) end },
              { type = 'Trigger', trigger = 'OnUnitDead',
                condition = function(ctx)
                  local dead = ctx:deadUnit()
                  return dead.ownerId == ctx.ownerId and ctx:tagOf(dead.cardId) == 'Hero'
                end,
                operation = function(ctx)
                  local dead = ctx:deadUnit()
                  ctx:summon('grave', dead.x, dead.y, dead.cardId)
                end },
            }";

        // 카드 ② 자기희생: 턴당 1회 스스로를 릴리스하고 자원 +1.
        private const string MartyrLua = @"
            return {
              { type = 'Ignition', usesPerTurn = 1,
                operation = function(ctx)
                  ctx:release(ctx.sourceUnitId)
                  ctx:changeAbilityResource(1)
                end },
            }";

        // 카드 ③ 부활: 자원 2를 쓰고 아군 무덤을 골라, 무덤을 릴리스하고 그 칸에 원래 유닛을 소환.
        private const string NecromancerLua = @"
            return {
              { type = 'Ignition', usesPerTurn = 1,
                cost = { abilityResource = 2 },
                target = function(ctx) return ctx:findUnits('Self', 'Graveyard') end,
                operation = function(ctx, targets)
                  for _, t in ipairs(targets) do
                    ctx:release(t.unitId)
                    if t.originCardId then ctx:summon(t.originCardId, t.x, t.y) end
                  end
                end },
            }";

        private const string BankerLua = @"
            return {
              { type = 'Ignition', usesPerTurn = 1, operation = function(ctx) ctx:changeAbilityResource(2) end },
            }";

        private const string DrummerLua = @"
            return {
              { type = 'Ignition', usesPerTurn = 1, operation = function(ctx) ctx:changeAbilityResource(1) end },
            }";

        private const string AvengerLua = @"
            return {
              { type = 'Trigger', trigger = 'OnUnitDead',
                condition = function(ctx)
                  local dead = ctx:deadUnit()
                  return dead.ownerId ~= ctx.ownerId and ctx:tagOf(dead.cardId) == 'Hero'
                end,
                operation = function(ctx) ctx:release(ctx.sourceUnitId) end },
            }";

        private const string GravediggerLua = @"
            return {
              { type = 'Trigger', trigger = 'OnUnitDead',
                operation = function(ctx)
                  local dead = ctx:deadUnit()
                  ctx:summon('grave', dead.x, dead.y, dead.cardId)
                end },
            }";

        // 카드 ⑥ 범위 공격: 행동력 1을 쓰고 상대 영웅을 골라, 그 칸과 상하좌우 1칸의 상대 유닛에게 CP만큼 피해.
        private const string BomberLua = @"
            return {
              { type = 'Ignition', usesPerTurn = 1,
                cost = { actionResource = 1 },
                target = function(ctx) return ctx:findUnits('Opponent', 'Hero') end,
                operation = function(ctx, targets)
                  for _, t in ipairs(targets) do
                    ctx:damageArea(t.x, t.y, { {0, 0}, {0, 1}, {0, -1}, {1, 0}, {-1, 0} })
                  end
                end },
            }";

        // 카드 ④ 지연 파괴: 자신에게 2턴 상태 이상을 걸고, 만료되면 자신을 파괴.
        private const string DoomedLua = @"
            return {
              { type = 'Ignition', usesPerTurn = 1,
                operation = function(ctx)
                  ctx:addStatus(ctx.sourceUnitId, { turns = 2,
                    onExpire = function(ctx) ctx:destroy(ctx.sourceUnitId) end })
                end },
            }";

        // 카드 ⑦ 보호막: 자원 2를 쓰고 아군 1명에게 2턴 보호막.
        private const string GuardianLua = @"
            return {
              { type = 'Ignition', usesPerTurn = 1,
                cost = { abilityResource = 2 },
                target = function(ctx) return ctx:findUnits('Self', nil) end,
                operation = function(ctx, targets)
                  for _, t in ipairs(targets) do ctx:addStatus(t.unitId, { type = 'Shield', turns = 2 }) end
                end },
            }";

        // 비용 없이 자신에게 영구 보호막. 겹쳐 거는 테스트용.
        private const string BlessedLua = @"
            return {
              { type = 'Ignition', operation = function(ctx) ctx:addStatus(ctx.sourceUnitId, { type = 'Shield' }) end },
            }";

        // 1턴 뒤 자원 +1. 같은 효과가 교체되지 않고 쌓이는지 확인한다.
        private const string TickerLua = @"
            return {
              { type = 'Ignition',
                operation = function(ctx)
                  ctx:addStatus(ctx.sourceUnitId, { turns = 1, onExpire = function(ctx) ctx:changeAbilityResource(1) end })
                end },
            }";

        private const string SmiterLua = @"
            return {
              { type = 'Ignition', usesPerTurn = 1,
                target = function(ctx) return ctx:findUnits('Opponent', nil) end,
                operation = function(ctx, targets)
                  for _, t in ipairs(targets) do ctx:destroy(t.unitId) end
                end },
            }";

        private const string TypoLua = @"
            return {
              { type = 'Ignition', operation = function(ctx) ctx:addStatus(ctx.sourceUnitId, { type = 'Sheild' }) end },
            }";

        // 카드 ⑤ 공격력 오라: 내 유닛 공격력 +1.
        private const string AuraLua = @"
            return {
              { type = 'Continuous', cp = 1,
                target = function(ctx) return ctx:findUnits('Self', nil) end },
            }";

        // 상대 유닛 하나에 2턴 동안 CP -3.
        private const string WeakenerLua = @"
            return {
              { type = 'Ignition', usesPerTurn = 1,
                target = function(ctx) return ctx:findUnits('Opponent', nil) end,
                operation = function(ctx, targets)
                  for _, t in ipairs(targets) do ctx:addStatus(t.unitId, { cp = -3, turns = 2 }) end
                end },
            }";

        // 자신에게 영구 CP -20. CP가 0 아래로 내려가지 않는지 확인한다.
        private const string CursedLua = @"
            return {
              { type = 'Ignition', operation = function(ctx) ctx:addStatus(ctx.sourceUnitId, { cp = -20 }) end },
            }";

        private GameEngine engine;

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
        public void ReviveWaitsForChoiceThenSummonsOriginalHero()
        {
            StartGame(Place(1, "watcher", 0, 0), Place(1, "hero", 2, 1), Place(2, "fighter", 2, 0),
                Place(1, "necromancer", 6, 6), Place(1, "banker", 0, 6));
            SubmitOk(1, new EndTurnCommand());
            SubmitOk(2, new AttackCommand(3, 2));
            SubmitOk(2, new EndTurnCommand());
            int graveId = engine.State.Board.GetUnitIdAt(2, 1).Value;

            SubmitResult poor = engine.Submit(1, new ActivateEffectCommand(4, 0));
            Assert.IsFalse(poor.Succeeded);
            StringAssert.Contains("비용", poor.Reason);

            SubmitOk(1, new ActivateEffectCommand(5, 0));
            SubmitResult activated = engine.Submit(1, new ActivateEffectCommand(4, 0));

            Assert.IsTrue(activated.Succeeded, activated.Reason);
            Assert.AreEqual(0, engine.State.Players[1].AbilityResource);
            ChoiceRequested requested = activated.Events.OfType<ChoiceRequested>().Single();
            Assert.AreEqual(1, requested.VisibleTo);
            CollectionAssert.AreEqual(new[] { graveId }, requested.Choice.Candidates);

            // 선택하기 전에는 다른 명령과 잘못된 선택을 받지 않는다.
            Assert.IsFalse(engine.Submit(1, new EndTurnCommand()).Succeeded);
            Assert.IsFalse(engine.Submit(1, new ChooseCommand(3)).Succeeded);
            Assert.IsFalse(engine.Submit(2, new ChooseCommand(graveId)).Succeeded);
            Assert.IsEmpty(engine.GetAvailableCommands(2));
            Assert.AreEqual(graveId, ((ChooseCommand)engine.GetAvailableCommands(1).Single()).UnitIds.Single());

            SubmitResult chosen = engine.Submit(1, new ChooseCommand(graveId));

            Assert.IsTrue(chosen.Succeeded, chosen.Reason);
            Assert.AreEqual(DeathCause.Release, chosen.Events.OfType<UnitDied>().Single().Cause);
            UnitSummoned revived = chosen.Events.OfType<UnitSummoned>().Single();
            Assert.AreEqual("hero", revived.CardId);
            Assert.AreEqual(1, revived.OwnerId);
            Assert.AreEqual(new GridPos(2, 1), revived.Position);
            // 새 ID를 받으므로 트리거 순서가 가장 나중이 된다(§4.2).
            Assert.Greater(revived.UnitId, graveId);
            Assert.IsNull(engine.State.PendingChoice);
            SubmitOk(1, new EndTurnCommand());
        }

        [Test]
        public void ReviveCannotBeUsedWithoutAllyGrave()
        {
            StartGame(Place(1, "necromancer", 6, 6), Place(1, "banker", 0, 6), Place(2, "grave", 3, 3));
            SubmitOk(1, new ActivateEffectCommand(2, 0));

            SubmitResult result = engine.Submit(1, new ActivateEffectCommand(1, 0));

            Assert.IsFalse(result.Succeeded);
            StringAssert.Contains("대상", result.Reason);
            Assert.AreEqual(2, engine.State.Players[1].AbilityResource);
            Assert.IsFalse(engine.GetAvailableCommands(1).OfType<ActivateEffectCommand>().Any(c => c.UnitId == 1));
        }

        [Test]
        public void LuaSyntaxErrorReportsFileAndLine()
        {
            InvalidDataException exception = LoadFails("return { { type = 'Ignition', operation = function(ctx) ctx:release( end } }");

            StringAssert.Contains("broken.lua:(1,", exception.Message);
        }

        [Test]
        public void MisspelledEffectKeyFailsToLoad()
        {
            InvalidDataException exception = LoadFails(
                "return { { type = 'Ignition', usesPerturn = 1, operation = function(ctx) end } }");

            StringAssert.Contains("broken.lua: [1]", exception.Message);
            StringAssert.Contains("usesPerturn", exception.Message);
        }

        [Test]
        public void LuaWithoutMatchingJsonFailsToLoad()
        {
            InvalidDataException exception = Assert.Throws<InvalidDataException>(
                () => CardLoader.Load(new[] { ("orphan.lua", "return {}") }));

            StringAssert.Contains("orphan.lua", exception.Message);
        }

        [Test]
        public void ConditionCannotChangeState()
        {
            CardDatabase database = new(CardLoader.Load(Card("cheater", "Normal",
                "return { { type = 'Ignition', condition = function(ctx) ctx:changeAbilityResource(5) return true end, operation = function(ctx) end } }")));
            engine = new GameEngine(new GameState(new PlayerState(1), new PlayerState(2)), database);
            Assert.IsTrue(engine.StartGame(new[] { Place(1, "cheater", 0, 0), Place(2, "cheater", 4, 6) }, out string reason), reason);

            Assert.Throws<System.InvalidOperationException>(() => engine.GetAvailableCommands(1));
            Assert.AreEqual(0, engine.State.Players[1].AbilityResource);
        }

        [Test]
        public void AreaAttackDamagesOpponentsInRangeAndTheyDieTogether()
        {
            // 1 폭격수, 2 상대 영웅(중심), 3 상대(범위 안), 4 상대(범위 밖), 5 아군(범위 안)
            StartGame(Place(1, "bomber", 0, 0), Place(2, "hero", 3, 3), Place(2, "fighter", 3, 4),
                Place(2, "fighter", 5, 5), Place(1, "fighter", 2, 3));

            SubmitResult activated = engine.Submit(1, new ActivateEffectCommand(1, 0));

            Assert.IsTrue(activated.Succeeded, activated.Reason);
            Assert.AreEqual(GameEngine.ActionsPerTurn - 1, engine.State.Players[1].ActionResource);
            CollectionAssert.AreEqual(new[] { 2 }, activated.Events.OfType<ChoiceRequested>().Single().Choice.Candidates);

            SubmitResult chosen = engine.Submit(1, new ChooseCommand(2));

            Assert.IsTrue(chosen.Succeeded, chosen.Reason);
            CollectionAssert.AreEqual(new[] { 2, 3 }, chosen.Events.OfType<UnitDamaged>().Select(e => e.UnitId).ToArray());
            CollectionAssert.AreEqual(new[] { 2, 3 }, chosen.Events.OfType<UnitDied>().Select(e => e.UnitId).ToArray());
            // 피해를 모두 준 뒤에 함께 사망한다.
            int lastDamaged = chosen.Events.ToList().FindLastIndex(e => e is UnitDamaged);
            int firstDied = chosen.Events.ToList().FindIndex(e => e is UnitDied);
            Assert.Less(lastDamaged, firstDied);
            Assert.AreEqual(10, engine.State.Units[4].CurrentHP);
            Assert.AreEqual(10, engine.State.Units[5].CurrentHP);
            Assert.AreEqual(10, engine.State.Units[1].CurrentHP); // 반격 없음
        }

        [Test]
        public void AreaAttackNeedsActionResource()
        {
            StartGame(Place(1, "bomber", 0, 0), Place(2, "hero", 3, 3));
            SubmitOk(1, new MoveCommand(1, new GridPos(0, 1)));
            SubmitOk(1, new MoveCommand(1, new GridPos(0, 2)));

            SubmitResult result = engine.Submit(1, new ActivateEffectCommand(1, 0));

            Assert.IsFalse(result.Succeeded);
            StringAssert.Contains("비용", result.Reason);
            Assert.IsFalse(engine.GetAvailableCommands(1).OfType<ActivateEffectCommand>().Any());
        }

        [Test]
        public void DelayedDestroyFiresAfterTwoOwnTurns()
        {
            StartGame(Place(1, "doomed", 0, 0), Place(2, "fighter", 6, 6));
            SubmitOk(1, new ActivateEffectCommand(1, 0));

            PassRound();
            Assert.IsTrue(engine.State.Units.ContainsKey(1));
            Assert.AreEqual(1, engine.State.Units[1].Statuses.Single().RemainingTurns);

            SubmitResult result = PassRound();

            UnitDied died = result.Events.OfType<UnitDied>().Single();
            Assert.AreEqual(1, died.UnitId);
            Assert.AreEqual(DeathCause.Destroy, died.Cause);
            Assert.AreEqual(1, died.KillerPlayerId);
            Assert.IsFalse(engine.State.Units.ContainsKey(1));
        }

        [Test]
        public void DelayedEffectsStackInsteadOfReplacing()
        {
            StartGame(Place(1, "ticker", 0, 0), Place(2, "fighter", 6, 6));
            SubmitOk(1, new ActivateEffectCommand(1, 0));
            SubmitOk(1, new ActivateEffectCommand(1, 0));
            Assert.AreEqual(2, engine.State.Units[1].Statuses.Count);

            PassRound();

            Assert.AreEqual(2, engine.State.Players[1].AbilityResource);
            Assert.IsEmpty(engine.State.Units[1].Statuses);
        }

        [Test]
        public void ShieldBlocksOneHitThenBreaks()
        {
            // 1 수호자, 2 은행가, 3 아군(보호 대상), 4 상대
            StartGame(Place(1, "guardian", 0, 0), Place(1, "banker", 0, 6), Place(1, "fighter", 3, 3),
                Place(2, "fighter", 3, 4));
            SubmitOk(1, new ActivateEffectCommand(2, 0));
            SubmitOk(1, new ActivateEffectCommand(1, 0));
            SubmitOk(1, new ChooseCommand(3));
            Assert.AreEqual(0, engine.State.Players[1].AbilityResource);
            Assert.IsTrue(engine.State.Units[3].HasStatus(StatusType.Shield));
            SubmitOk(1, new EndTurnCommand());

            SubmitResult blocked = engine.Submit(2, new AttackCommand(4, 3));

            Assert.IsTrue(blocked.Succeeded, blocked.Reason);
            Assert.AreEqual(0, blocked.Events.OfType<UnitDamaged>().Single().Amount);
            Assert.AreEqual(10, engine.State.Units[3].CurrentHP);
            Assert.AreEqual(3, blocked.Events.OfType<StatusRemoved>().Single().UnitId);
            Assert.IsFalse(engine.State.Units[3].HasStatus(StatusType.Shield));

            SubmitResult second = engine.Submit(2, new AttackCommand(4, 3));

            Assert.IsTrue(second.Succeeded, second.Reason);
            Assert.AreEqual(3, second.Events.OfType<UnitDied>().Single().UnitId);
        }

        [Test]
        public void StackedShieldsBlockOnlyOnce()
        {
            StartGame(Place(1, "blessed", 3, 3), Place(2, "fighter", 3, 4));
            SubmitOk(1, new ActivateEffectCommand(1, 0));
            SubmitOk(1, new ActivateEffectCommand(1, 0));
            SubmitOk(1, new EndTurnCommand());

            SubmitResult blocked = engine.Submit(2, new AttackCommand(2, 1));

            Assert.IsTrue(blocked.Succeeded, blocked.Reason);
            Assert.AreEqual(2, blocked.Events.OfType<StatusRemoved>().Count());
            Assert.IsEmpty(engine.State.Units[1].Statuses);

            SubmitResult second = engine.Submit(2, new AttackCommand(2, 1));
            Assert.AreEqual(1, second.Events.OfType<UnitDied>().Single().UnitId);
        }

        [Test]
        public void ShieldExpiresAfterTwoTurnsWithoutHit()
        {
            StartGame(Place(1, "guardian", 0, 0), Place(1, "banker", 0, 6), Place(2, "fighter", 6, 6));
            SubmitOk(1, new ActivateEffectCommand(2, 0));
            SubmitOk(1, new ActivateEffectCommand(1, 0));
            SubmitOk(1, new ChooseCommand(2));

            PassRound();
            Assert.IsTrue(engine.State.Units[2].HasStatus(StatusType.Shield));

            SubmitResult result = PassRound();

            Assert.AreEqual(2, result.Events.OfType<StatusRemoved>().Single().UnitId);
            Assert.IsFalse(engine.State.Units[2].HasStatus(StatusType.Shield));
        }

        [Test]
        public void ShieldDoesNotStopDestroy()
        {
            StartGame(Place(1, "blessed", 0, 0), Place(2, "smiter", 6, 6));
            SubmitOk(1, new ActivateEffectCommand(1, 0));
            SubmitOk(1, new EndTurnCommand());
            SubmitOk(2, new ActivateEffectCommand(2, 0));

            SubmitResult result = engine.Submit(2, new ChooseCommand(1));

            Assert.IsTrue(result.Succeeded, result.Reason);
            Assert.IsFalse(result.Events.OfType<UnitDamaged>().Any());
            UnitDied died = result.Events.OfType<UnitDied>().Single();
            Assert.AreEqual(DeathCause.Destroy, died.Cause);
            Assert.AreEqual(2, died.KillerPlayerId);
        }

        [Test]
        public void UnknownStatusTypeIsScriptError()
        {
            StartGame(Place(1, "typo", 0, 0), Place(2, "fighter", 6, 6));

            System.InvalidOperationException exception = Assert.Throws<System.InvalidOperationException>(
                () => engine.Submit(1, new ActivateEffectCommand(1, 0)));
            StringAssert.Contains("Sheild", exception.Message);
        }

        [Test]
        public void AuraRaisesAllyCPOnly()
        {
            StartGame(Place(1, "aura", 0, 0), Place(1, "fighter", 3, 3), Place(2, "fighter", 3, 4));

            Assert.AreEqual(11, engine.GetCP(1));
            Assert.AreEqual(11, engine.GetCP(2));
            Assert.AreEqual(10, engine.GetCP(3));
        }

        [Test]
        public void AurasStack()
        {
            StartGame(Place(1, "aura", 0, 0), Place(1, "aura", 6, 0), Place(1, "fighter", 3, 3), Place(2, "fighter", 6, 6));

            Assert.AreEqual(12, engine.GetCP(3));
        }

        [Test]
        public void AuraDeathRemovesBonusAndReportsChange()
        {
            // 1 오라, 2 아군, 3 상대
            StartGame(Place(1, "aura", 3, 3), Place(1, "fighter", 0, 0), Place(2, "fighter", 3, 4));
            SubmitOk(1, new EndTurnCommand());

            SubmitResult result = engine.Submit(2, new AttackCommand(3, 1));

            Assert.IsTrue(result.Succeeded, result.Reason);
            Assert.AreEqual(10, engine.GetCP(2));
            UnitCPChanged changed = result.Events.OfType<UnitCPChanged>().Single();
            Assert.AreEqual(2, changed.UnitId);
            Assert.AreEqual(10, changed.CP);
        }

        [Test]
        public void CPStatusAppliesUntilExpired()
        {
            StartGame(Place(1, "weakener", 3, 3), Place(2, "fighter", 3, 4));
            SubmitOk(1, new ActivateEffectCommand(1, 0));

            SubmitResult chosen = engine.Submit(1, new ChooseCommand(2));

            Assert.IsTrue(chosen.Succeeded, chosen.Reason);
            UnitCPChanged weakened = chosen.Events.OfType<UnitCPChanged>().Single();
            Assert.AreEqual(2, weakened.UnitId);
            Assert.AreEqual(7, weakened.CP);

            SubmitOk(1, new EndTurnCommand());
            SubmitResult attack = engine.Submit(2, new AttackCommand(2, 1));
            Assert.AreEqual(7, attack.Events.OfType<UnitDamaged>().Single().Amount);

            // 상태 이상은 붙은 유닛의 주인(플레이어 2) 턴이 두 번 시작되면 만료된다.
            SubmitOk(2, new EndTurnCommand());
            SubmitResult expired = engine.Submit(1, new EndTurnCommand());

            Assert.AreEqual(10, expired.Events.OfType<UnitCPChanged>().Single().CP);
            Assert.AreEqual(10, engine.GetCP(2));
        }

        [Test]
        public void CPDoesNotGoBelowZero()
        {
            StartGame(Place(1, "cursed", 3, 3), Place(2, "fighter", 3, 4));
            SubmitOk(1, new ActivateEffectCommand(1, 0));

            Assert.AreEqual(0, engine.GetCP(1));
            Assert.IsFalse(engine.CanAttackUnit(1, 1, 2, out string reason));
            StringAssert.Contains("공격력", reason);
        }

        [TestCase("return { { type = 'Continuous', target = function(ctx) return {} end } }", "cp")]
        [TestCase("return { { type = 'Continuous', cp = 1, target = function(ctx) return {} end, operation = function(ctx) end } }", "operation")]
        [TestCase("return { { type = 'Ignition', cp = 1, operation = function(ctx) end } }", "Continuous")]
        public void InvalidContinuousEffectFailsToLoad(string lua, string expected)
        {
            InvalidDataException exception = LoadFails(lua);

            StringAssert.Contains(expected, exception.Message);
        }

        // 상하좌우 1칸을 이동·공격할 수 있는 CP 10 카드.
        private static string UnitJson(string id, string tag) => $@"{{
            ""id"": ""{id}"", ""name"": ""{id}"", ""tag"": ""{tag}"",
            ""level"": 1, ""baseCP"": 10, ""baseHP"": 10,
            ""movement"": {{ ""type"": ""Line"", ""directions"": ""Orthogonal"", ""maxDistance"": 1 }},
            ""attack"": {{ ""directions"": ""Orthogonal"", ""maxDistance"": 1 }}
        }}";

        // 효과가 있는 카드는 JSON과 같은 이름의 Lua 파일을 함께 넘긴다.
        private static IEnumerable<(string, string)> Card(string id, string tag, string lua = null)
        {
            yield return ($"{id}.json", UnitJson(id, tag));
            if (lua != null)
                yield return ($"{id}.lua", lua);
        }

        private void StartGame(params UnitPlacement[] placements)
        {
            CardDatabase database = new(CardLoader.Load(
                Card("fighter", "Normal")
                    .Concat(Card("hero", "Hero"))
                    .Concat(Card("grave", "Graveyard"))
                    .Concat(Card("watcher", "Normal", WatcherLua))
                    .Concat(Card("martyr", "Normal", MartyrLua))
                    .Concat(Card("drummer", "Normal", DrummerLua))
                    .Concat(Card("avenger", "Normal", AvengerLua))
                    .Concat(Card("gravedigger", "Normal", GravediggerLua))
                    .Concat(Card("necromancer", "Normal", NecromancerLua))
                    .Concat(Card("banker", "Normal", BankerLua))
                    .Concat(Card("bomber", "Normal", BomberLua))
                    .Concat(Card("doomed", "Normal", DoomedLua))
                    .Concat(Card("guardian", "Normal", GuardianLua))
                    .Concat(Card("blessed", "Normal", BlessedLua))
                    .Concat(Card("ticker", "Normal", TickerLua))
                    .Concat(Card("smiter", "Normal", SmiterLua))
                    .Concat(Card("typo", "Normal", TypoLua))
                    .Concat(Card("aura", "Normal", AuraLua))
                    .Concat(Card("weakener", "Normal", WeakenerLua))
                    .Concat(Card("cursed", "Normal", CursedLua))));
            engine = new GameEngine(new GameState(new PlayerState(1), new PlayerState(2)), database);
            Assert.IsTrue(engine.StartGame(placements, out string reason), reason);
        }

        private static UnitPlacement Place(int ownerId, string cardId, int x, int y)
            => new UnitPlacement(ownerId, cardId, new GridPos(x, y));

        private void SubmitOk(int playerId, GameCommand command)
        {
            SubmitResult result = engine.Submit(playerId, command);
            Assert.IsTrue(result.Succeeded, result.Reason);
        }

        // 양쪽이 한 번씩 턴을 넘겨 플레이어 1의 턴으로 돌아온다. 마지막 턴 종료 결과를 돌려준다.
        private SubmitResult PassRound()
        {
            SubmitOk(1, new EndTurnCommand());
            SubmitResult result = engine.Submit(2, new EndTurnCommand());
            Assert.IsTrue(result.Succeeded, result.Reason);
            return result;
        }

        private static InvalidDataException LoadFails(string lua)
            => Assert.Throws<InvalidDataException>(() => CardLoader.Load(Card("broken", "Normal", lua)));
    }
}
