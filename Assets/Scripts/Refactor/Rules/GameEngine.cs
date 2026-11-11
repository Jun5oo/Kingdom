using System;
using System.Collections.Generic;

namespace Refactor
{
    // 행동 요청의 규칙을 검증하고 GameState에 반영하는 진입점.
    public sealed class GameEngine
    {
        // 턴 시작 시 지급하는 행동 횟수
        public const int ActionsPerTurn = 2;

        // 게임의 정보를 담고 있는 객체 
        public GameState State { get; }
        // 카드 데이터베이스 
        private readonly CardDatabase cardDatabase;
        // 이동 판별기 
        private readonly MovementValidator movementValidator = new();
        // 공격 처리 
        private readonly CombatResolver combatResolver = new();
        private bool isStarted;

        public GameEngine(GameState state, CardDatabase cardDatabase)
        {
            State = state ?? throw new ArgumentNullException(nameof(state));
            this.cardDatabase = cardDatabase ?? throw new ArgumentNullException(nameof(cardDatabase));
        }

        // 시작 유닛을 배치하고 선공에게 행동을 지급한다. 실패 시 상태를 변경하지 않는다.
        // shortcut: placements는 손패와 소환이 없어 유닛을 올릴 수단이 없는 동안의 임시 입력이다. 7단계에서 빈 보드와 킹 배치 단계로 시작하도록 바꾼다.
        public bool StartGame(IReadOnlyList<UnitPlacement> placements, out string reason)
        {
            if (placements == null)
                throw new ArgumentNullException(nameof(placements));

            if (isStarted)
            {
                reason = "이미 시작된 게임입니다.";
                return false;
            }

            List<UnitLevelData> levels = new(placements.Count);
            HashSet<(int x, int y)> reservedCells = new();

            foreach (UnitPlacement placement in placements)
            {
                int x = placement.Position.X;
                int y = placement.Position.Y;

                if (!State.Players.ContainsKey(placement.OwnerId))
                {
                    reason = $"등록되지 않은 플레이어입니다: {placement.OwnerId}";
                    return false;
                }

                if (!State.Board.Contains(x, y) || State.Board.GetUnitIdAt(x, y).HasValue ||
                    !reservedCells.Add((x, y)))
                {
                    reason = $"유닛을 배치할 수 없는 칸입니다: ({x}, {y})";
                    return false;
                }

                if (!cardDatabase.TryGetCard(placement.CardId, out CardDefinition card))
                {
                    reason = $"등록되지 않은 카드입니다: {placement.CardId}";
                    return false;
                }

                if (!card.TryGetLevel(placement.Level, out UnitLevelData levelData))
                {
                    reason = $"카드에 해당 등급이 없습니다: {placement.CardId}, {placement.Level}성";
                    return false;
                }

                if (levelData.BaseHP < 1)
                {
                    reason = $"카드의 Base HP가 1 이상이어야 합니다: {placement.CardId}";
                    return false;
                }

                levels.Add(levelData);
            }

            // 검증이 끝난 뒤에만 ID를 발급한다.
            List<UnitState> units = new(placements.Count);
            for (int i = 0; i < placements.Count; i++)
            {
                units.Add(new UnitState(State.AllocateUnitId(), placements[i].CardId, placements[i].OwnerId,
                    levels[i].BaseCP, levels[i].BaseHP, placements[i].Level));
            }

            for (int i = 0; i < units.Count; i++)
            {
                if (State.TrySpawnUnit(units[i], placements[i].Position.X, placements[i].Position.Y))
                    continue;

                // 앞서 배치한 유닛을 되돌려 실패 시 상태가 남지 않게 한다.
                for (int spawned = 0; spawned < i; spawned++)
                    State.TryRemoveUnit(units[spawned].Id);

                reason = "시작 유닛을 배치할 수 없습니다.";
                return false;
            }

            State.Players[State.CurrentPlayerId].ActionResource = ActionsPerTurn;
            isStarted = true;
            reason = string.Empty;
            return true;
        }

        // 모든 행동의 입구. 검증에 실패하면 상태를 변경하지 않는다.
        public SubmitResult Submit(int playerId, GameCommand command)
        {
            return command switch
            {
                MoveCommand move => SubmitMove(playerId, move),
                AttackCommand attack => SubmitAttack(playerId, attack),
                EndTurnCommand => SubmitEndTurn(playerId),
                ActivateEffectCommand activate => SubmitActivateEffect(playerId, activate),
                null => throw new ArgumentNullException(nameof(command)),
                _ => throw new ArgumentException($"처리할 수 없는 명령입니다: {command.GetType().Name}", nameof(command)),
            };
        }

        // 지금 실행할 수 있는 명령 전체. 현재 플레이어가 아니면 비어 있다.
        public IReadOnlyList<GameCommand> GetAvailableCommands(int playerId)
        {
            List<GameCommand> commands = new();
            if (!CanEndTurn(playerId, out _))
                return commands;

            commands.Add(new EndTurnCommand());
            foreach (UnitState unit in State.Units.Values)
            {
                if (unit.OwnerId == playerId)
                    AddUnitCommands(playerId, unit.Id, commands);
            }

            return commands;
        }

        public bool CanEndTurn(int playerId, out string reason)
        {
            if (!isStarted)
            {
                reason = "게임이 아직 시작되지 않았습니다.";
                return false;
            }

            if (!State.Players.ContainsKey(playerId))
            {
                reason = "등록되지 않은 플레이어입니다.";
                return false;
            }

            if (State.CurrentPlayerId != playerId)
            {
                reason = "자신의 턴에만 종료할 수 있습니다.";
                return false;
            }

            reason = string.Empty;
            return true;
        }

        // 유닛의 현재 등급에 해당하는 원본 데이터를 조회한다. 상태는 변경하지 않는다.
        public bool TryGetUnitLevelData(int unitId, out UnitLevelData levelData, out string reason)
        {
            levelData = null;

            if (!State.Units.TryGetValue(unitId, out UnitState unit))
            {
                reason = "유닛이 존재하지 않습니다.";
                return false;
            }

            if (!cardDatabase.TryGetCard(unit.CardDefinitionId, out CardDefinition card))
            {
                reason = $"등록되지 않은 카드입니다: {unit.CardDefinitionId}";
                return false;
            }

            if (!card.TryGetLevel(unit.Level, out levelData))
            {
                reason = $"카드에 해당 등급이 없습니다: {unit.CardDefinitionId}, {unit.Level}성";
                return false;
            }

            reason = string.Empty;
            return true;
        }

        // UI의 이동 미리보기에도 사용할 수 있는 읽기 전용 검증.
        public bool CanMoveUnit(int playerId, int unitId, int targetX, int targetY, out string reason)
        {
            if (!CanActWithUnit(playerId, unitId, out reason))
                return false;

            if (State.Players[playerId].ActionResource <= 0)
            {
                reason = "남은 행동 횟수가 없습니다.";
                return false;
            }

            if (!TryGetUnitLevelData(unitId, out UnitLevelData data, out reason))
                return false;

            if (data.Movement == null)
            {
                reason = "이동 규칙이 없습니다.";
                return false;
            }

            if (!State.Board.TryGetPosition(unitId, out int fromX, out int fromY))
            {
                reason = "유닛의 현재 위치를 찾을 수 없습니다.";
                return false;
            }

            return movementValidator.CanMove(
                State.Board, data.Movement,
                fromX, fromY, targetX, targetY, out reason);
        }
        // UI의 공격 미리보기에도 사용할 수 있는 읽기 전용 검증.
        public bool CanAttackUnit(int playerId, int attackerId, int defenderId, out string reason)
        {
            if (!CanActWithUnit(playerId, attackerId, out reason))
                return false;

            if (State.Players[playerId].ActionResource <= 0)
            {
                reason = "남은 행동 횟수가 없습니다.";
                return false;
            }

            if (!State.Units.TryGetValue(defenderId, out UnitState defender))
            {
                reason = "공격 대상이 존재하지 않습니다.";
                return false;
            }
            if (defender.OwnerId == playerId)
            {
                reason = "아군을 공격할 수 없습니다.";
                return false;
            }
            if (defender.CurrentHP <= 0)
            {
                reason = "이미 사망한 유닛입니다.";
                return false;
            }
            if (State.Units[attackerId].CP <= 0)
            {
                reason = "공격력이 없어 공격할 수 없습니다.";
                return false;
            }
            if (!TryGetUnitLevelData(attackerId, out UnitLevelData data, out reason))
                return false;

            if (!State.Board.TryGetPosition(attackerId, out int fromX, out int fromY) ||
                !State.Board.TryGetPosition(defenderId, out int targetX, out int targetY))
            {
                reason = "보드에서 공격 유닛을 찾을 수 없습니다.";
                return false;
            }

            return combatResolver.CanReach(State.Board, data.Attack,
                fromX, fromY, targetX, targetY, out reason);
        }
        // UI의 효과 버튼 표시에도 사용할 수 있는 읽기 전용 검증. 기동 효과만 직접 발동할 수 있다.
        public bool CanActivateEffect(int playerId, int unitId, int effectIndex, out string reason)
        {
            if (!CanActWithUnit(playerId, unitId, out reason))
                return false;

            if (!TryGetUnitLevelData(unitId, out UnitLevelData data, out reason))
                return false;

            if (effectIndex < 0 || effectIndex >= data.Effects.Count)
            {
                reason = "존재하지 않는 효과입니다.";
                return false;
            }

            EffectData effect = data.Effects[effectIndex];
            if (effect.Type != EffectType.Ignition)
            {
                reason = "직접 발동하는 효과가 아닙니다.";
                return false;
            }

            if (!State.Units[unitId].CanUseEffect(effectIndex, effect.UsesPerTurn))
            {
                reason = "이번 턴에는 더 사용할 수 없는 효과입니다.";
                return false;
            }

            // Condition은 상태를 바꾸지 않으므로 Processor 없이 확인한다.
            EffectContext context = new(State, cardDatabase, null, unitId, playerId, null);
            if (effect.Condition != null && !effect.Condition.CanExecute(context))
            {
                reason = "효과의 발동 조건을 만족하지 않습니다.";
                return false;
            }

            reason = string.Empty;
            return true;
        }

        // 유닛 행동의 공통 검증만 수행한다. 상태는 변경하지 않는다.
        // 이후 이동/공격 진입점에서 이 검증과 행동별 규칙을 모두 확인한 뒤 실행한다.
        // true는 이동 거리, 비용, 공격 대상까지 유효하다는 뜻이 아니다.
        public bool CanActWithUnit(int requestingPlayerId, int unitId, out string reason)
        {
            if (!State.Players.ContainsKey(requestingPlayerId))
            {
                reason = "등록되지 않은 플레이어입니다.";
                return false;
            }

            if (State.CurrentPlayerId != requestingPlayerId)
            {
                reason = "현재 행동할 플레이어가 아닙니다.";
                return false;
            }

            if (!State.Units.TryGetValue(unitId, out UnitState unit))
            {
                reason = "유닛이 존재하지 않습니다.";
                return false;
            }

            if (unit.OwnerId != requestingPlayerId)
            {
                reason = "자신의 유닛만 조작할 수 있습니다.";
                return false;
            }

            if (unit.CurrentHP <= 0)
            {
                reason = "사망한 유닛은 행동할 수 없습니다.";
                return false;
            }

            if (!State.Board.TryGetPosition(unitId, out _, out _))
            {
                reason = "보드에 배치되지 않은 유닛입니다.";
                return false;
            }

            reason = string.Empty;
            return true;
        }

        private void AddUnitCommands(int playerId, int unitId, List<GameCommand> commands)
        {
            for (int y = 0; y < State.Board.Height; y++)
            {
                for (int x = 0; x < State.Board.Width; x++)
                {
                    if (CanMoveUnit(playerId, unitId, x, y, out _))
                        commands.Add(new MoveCommand(unitId, new GridPos(x, y)));

                    int? targetId = State.Board.GetUnitIdAt(x, y);
                    if (targetId.HasValue && CanAttackUnit(playerId, unitId, targetId.Value, out _))
                        commands.Add(new AttackCommand(unitId, targetId.Value));
                }
            }

            if (!TryGetUnitLevelData(unitId, out UnitLevelData data, out _))
                return;

            for (int i = 0; i < data.Effects.Count; i++)
            {
                if (CanActivateEffect(playerId, unitId, i, out _))
                    commands.Add(new ActivateEffectCommand(unitId, i));
            }
        }

        private SubmitResult SubmitMove(int playerId, MoveCommand command)
        {
            GridPos target = command.Target;
            if (!CanMoveUnit(playerId, command.UnitId, target.X, target.Y, out string reason))
                return SubmitResult.Fail(reason);

            State.Board.TryGetPosition(command.UnitId, out int fromX, out int fromY);
            if (!State.TryMoveUnit(command.UnitId, target.X, target.Y))
                return SubmitResult.Fail("유닛을 이동할 수 없습니다.");

            EffectProcessor processor = new(State, cardDatabase);
            processor.Raise(new UnitMoved(command.UnitId, new GridPos(fromX, fromY), target));
            processor.ChangeActionResource(playerId, -1);
            processor.RunToCompletion();
            return SubmitResult.Ok(processor.Events);
        }

        private SubmitResult SubmitAttack(int playerId, AttackCommand command)
        {
            if (!CanAttackUnit(playerId, command.AttackerId, command.DefenderId, out string reason))
                return SubmitResult.Fail(reason);

            UnitState attacker = State.Units[command.AttackerId];
            UnitState defender = State.Units[command.DefenderId];

            EffectProcessor processor = new(State, cardDatabase);
            processor.Damage(defender.Id, combatResolver.CalculateDamage(attacker, defender), playerId);
            processor.ChangeActionResource(playerId, -1);
            processor.RunToCompletion();
            return SubmitResult.Ok(processor.Events);
        }

        // 기동 효과는 행동 횟수를 쓰지 않는다. 비용은 3단계에서 효과의 Cost로 처리한다.
        private SubmitResult SubmitActivateEffect(int playerId, ActivateEffectCommand command)
        {
            if (!CanActivateEffect(playerId, command.UnitId, command.EffectIndex, out string reason))
                return SubmitResult.Fail(reason);

            EffectProcessor processor = new(State, cardDatabase);
            processor.Activate(command.UnitId, command.EffectIndex);
            processor.RunToCompletion();
            return SubmitResult.Ok(processor.Events);
        }

        // 한 플레이어의 차례를 1턴으로 센다. 남은 행동은 이월하지 않는다.
        private SubmitResult SubmitEndTurn(int playerId)
        {
            if (!CanEndTurn(playerId, out string reason))
                return SubmitResult.Fail(reason);

            PlayerState nextPlayer = null;
            foreach (PlayerState player in State.Players.Values)
            {
                if (player.Id != playerId)
                    nextPlayer = player;
            }

            EffectProcessor processor = new(State, cardDatabase);
            processor.ChangeActionResource(playerId, -State.Players[playerId].ActionResource);
            State.CurrentPlayerId = nextPlayer.Id;
            State.TurnNumber += 1;
            foreach (UnitState unit in State.Units.Values)
                unit.ResetEffectUses();

            processor.Raise(new TurnStarted(nextPlayer.Id, State.TurnNumber));
            processor.ChangeActionResource(nextPlayer.Id, ActionsPerTurn - nextPlayer.ActionResource);
            processor.RunToCompletion();
            return SubmitResult.Ok(processor.Events);
        }
    }
}
