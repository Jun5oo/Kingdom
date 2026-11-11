using System;
using System.Collections.Generic;
using UnityEngine;

namespace Refactor
{
    // 입력을 명령으로 보내고, 엔진이 돌려준 이벤트를 화면에 재생한다.
    public sealed class GamePresenter : MonoBehaviour
    {
        private readonly Dictionary<int, UnitView> unitViews = new();
        private readonly Dictionary<int, GridPos> unitPositions = new();
        private readonly Dictionary<int, int> abilityResources = new();
        private IReadOnlyList<GameCommand> availableCommands = Array.Empty<GameCommand>();
        private GameEngine engine;
        private CardDatabase database;
        private BoardView boardView;
        private UnitView unitPrefab;
        private int? selectedUnitId;
        private int currentPlayerId;
        private int turnNumber;
        private int remainingActions;
        private string status = string.Empty;

        // 엔진 상태는 여기서만 읽는다. 이후 변화는 이벤트로만 받는다.
        public void Initialize(GameEngine engine, CardDatabase database, BoardView boardView, UnitView unitPrefab)
        {
            if (this.engine != null)
                throw new InvalidOperationException("이미 초기화되었습니다.");
            this.database = database ?? throw new ArgumentNullException(nameof(database));
            this.unitPrefab = unitPrefab != null ? unitPrefab : throw new ArgumentNullException(nameof(unitPrefab));
            this.boardView = boardView != null ? boardView : throw new ArgumentNullException(nameof(boardView));
            this.engine = engine ?? throw new ArgumentNullException(nameof(engine));
            this.boardView.CellClicked += OnCellClicked;

            GameState state = engine.State;
            // Show는 기존 칸을 다시 만들므로 유닛 UI 생성 전에 한 번 호출한다.
            this.boardView.Show(state.Board);
            foreach (UnitState unit in state.Units.Values)
            {
                if (!state.Board.TryGetPosition(unit.Id, out int x, out int y))
                    throw new InvalidOperationException($"보드에 없는 유닛입니다: {unit.Id}");

                CreateUnitView(unit.Id, unit.OwnerId, unit.CardDefinitionId, new GridPos(x, y),
                    unit.CP, unit.MaxHP, unit.CurrentHP);
            }

            foreach (PlayerState player in state.Players.Values)
                abilityResources[player.Id] = player.AbilityResource;

            currentPlayerId = state.CurrentPlayerId;
            turnNumber = state.TurnNumber;
            remainingActions = state.Players[currentPlayerId].ActionResource;
            availableCommands = engine.GetAvailableCommands(currentPlayerId);
            status = $"플레이어 {currentPlayerId}의 유닛을 선택해주세요.";
        }

        private void OnDestroy()
        {
            if (boardView != null)
                boardView.CellClicked -= OnCellClicked;
        }

        private void CreateUnitView(int unitId, int ownerId, string cardId, GridPos position,
            int cp, int maxHP, int currentHP)
        {
            if (!database.TryGetCard(cardId, out CardDefinition card))
                throw new InvalidOperationException($"등록되지 않은 카드입니다: {cardId}");

            UnitView view = boardView.CreateUnitView(unitPrefab, position.X, position.Y);
            view.Bind(unitId, ownerId, cp, maxHP, currentHP, card);
            unitViews.Add(unitId, view);
            unitPositions.Add(unitId, position);
        }

        private void OnCellClicked(int x, int y)
        {
            if (!isActiveAndEnabled)
                return;

            int? occupant = FindUnitAt(x, y);
            if (occupant.HasValue && unitViews[occupant.Value].OwnerId == currentPlayerId)
            {
                selectedUnitId = selectedUnitId == occupant ? null : occupant;
                status = selectedUnitId.HasValue ? "빈칸은 이동, 적 유닛은 공격입니다." : "선택을 해제했습니다.";
                RefreshHighlights();
                return;
            }

            if (!selectedUnitId.HasValue)
            {
                status = "먼저 아군 유닛을 선택해주세요.";
                return;
            }

            if (occupant.HasValue)
                SubmitCommand(new AttackCommand(selectedUnitId.Value, occupant.Value));
            else
                SubmitCommand(new MoveCommand(selectedUnitId.Value, new GridPos(x, y)));
        }

        private int? FindUnitAt(int x, int y)
        {
            foreach (KeyValuePair<int, GridPos> entry in unitPositions)
            {
                if (entry.Value.X == x && entry.Value.Y == y)
                    return entry.Key;
            }

            return null;
        }

        private void SubmitCommand(GameCommand command)
        {
            SubmitResult result = engine.Submit(currentPlayerId, command);
            if (!result.Succeeded)
            {
                status = result.Reason;
                return;
            }

            foreach (GameEvent gameEvent in result.Events)
                Play(gameEvent);

            if (remainingActions == 0)
            {
                status = command is MoveCommand
                    ? "행동을 모두 사용했습니다. 턴 종료 버튼을 눌러주세요."
                    : status + " 턴 종료 버튼을 눌러주세요.";
            }

            availableCommands = engine.GetAvailableCommands(currentPlayerId);
            RefreshHighlights();
        }

        private void Play(GameEvent gameEvent)
        {
            switch (gameEvent)
            {
                case UnitMoved moved:
                    boardView.MoveUnitView(unitViews[moved.UnitId], moved.To.X, moved.To.Y);
                    unitPositions[moved.UnitId] = moved.To;
                    status = "이동했습니다. 같은 유닛으로 한 번 더 행동할 수 있습니다.";
                    break;

                case UnitDamaged damaged:
                    unitViews[damaged.UnitId].SetHP(damaged.RemainingHP);
                    status = damaged.RemainingHP > 0
                        ? $"{damaged.Amount} 피해. 상대 HP: {damaged.RemainingHP}"
                        : $"{damaged.Amount} 피해. 적을 처치했습니다.";
                    break;

                case UnitDied died:
                    UnitView view = unitViews[died.UnitId];
                    view.gameObject.SetActive(false);
                    Destroy(view.gameObject);
                    unitViews.Remove(died.UnitId);
                    unitPositions.Remove(died.UnitId);
                    break;

                case UnitSummoned summoned:
                    CreateUnitView(summoned.UnitId, summoned.OwnerId, summoned.CardId, summoned.Position,
                        summoned.CP, summoned.HP, summoned.HP);
                    break;

                case ActionResourceChanged changed:
                    // 효과로 상대 자원이 바뀔 수 있으므로 현재 플레이어 것만 반영한다.
                    if (changed.PlayerId == currentPlayerId)
                        remainingActions = changed.Remaining;
                    break;

                case AbilityResourceChanged ability:
                    abilityResources[ability.PlayerId] = ability.Remaining;
                    break;

                case TurnStarted started:
                    currentPlayerId = started.PlayerId;
                    turnNumber = started.TurnNumber;
                    selectedUnitId = null;
                    status = $"플레이어 {started.PlayerId}의 턴입니다.";
                    break;

                default:
                    throw new ArgumentException($"재생할 수 없는 이벤트입니다: {gameEvent.GetType().Name}", nameof(gameEvent));
            }
        }

        private void RefreshHighlights()
        {
            boardView.ClearHighlights();
            if (!selectedUnitId.HasValue)
                return;

            int unitId = selectedUnitId.Value;
            foreach (GameCommand command in availableCommands)
            {
                if (command is MoveCommand move && move.UnitId == unitId)
                {
                    boardView.HighlightCell(move.Target.X, move.Target.Y, CellHighlight.Move);
                }
                else if (command is AttackCommand attack && attack.AttackerId == unitId)
                {
                    GridPos target = unitPositions[attack.DefenderId];
                    boardView.HighlightCell(target.X, target.Y, CellHighlight.Attack);
                }
            }

            GridPos selected = unitPositions[unitId];
            boardView.HighlightCell(selected.X, selected.Y, CellHighlight.Selected);
        }

        // 임시 HUD. 별도 HUD View를 만들면 옮긴다.
        private void OnGUI()
        {
            if (engine == null)
                return;

            GUILayout.BeginArea(new Rect(12, 12, Mathf.Min(520, Screen.width - 24), 150), GUI.skin.box);
            GUILayout.Label($"{turnNumber}턴 · 플레이어 {currentPlayerId}");
            GUILayout.Label($"남은 행동: {remainingActions} / {GameEngine.ActionsPerTurn} · 능력 자원: {abilityResources[currentPlayerId]}");
            GUILayout.Label(status);
            if (GUILayout.Button("턴 종료"))
                SubmitCommand(new EndTurnCommand());
            GUILayout.EndArea();
        }
    }
}
