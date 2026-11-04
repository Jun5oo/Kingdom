using System;
using System.Collections.Generic;
using UnityEngine;

namespace Refactor
{
    // 보드 입력을 엔진에 전달하고 결과를 화면에 반영한다.
    public sealed class GamePresenter : MonoBehaviour
    {
        private readonly Dictionary<int, UnitView> unitViews = new();
        private GameEngine engine;
        private BoardView boardView;
        private int? selectedUnitId;
        private string status = string.Empty;

        private GameState State => engine.State;

        public void Initialize(GameEngine engine, CardDatabase database, BoardView boardView, UnitView unitPrefab)
        {
            if (this.engine != null)
                throw new InvalidOperationException("이미 초기화되었습니다.");
            if (database == null)
                throw new ArgumentNullException(nameof(database));
            if (unitPrefab == null)
                throw new ArgumentNullException(nameof(unitPrefab));

            this.boardView = boardView != null ? boardView : throw new ArgumentNullException(nameof(boardView));
            this.engine = engine ?? throw new ArgumentNullException(nameof(engine));
            this.boardView.CellClicked += OnCellClicked;

            // Show는 기존 칸을 다시 만들므로 유닛 UI 생성 전에 한 번 호출한다.
            this.boardView.Show(State.Board);
            foreach (UnitState unit in State.Units.Values)
                CreateUnitView(unit, database, unitPrefab);

            status = $"플레이어 {State.CurrentPlayerId}의 유닛을 선택해주세요.";
        }

        private void OnDestroy()
        {
            if (boardView != null)
                boardView.CellClicked -= OnCellClicked;
        }

        private void CreateUnitView(UnitState unit, CardDatabase database, UnitView unitPrefab)
        {
            if (!database.TryGetCard(unit.CardDefinitionId, out UnitCardData card))
                throw new InvalidOperationException($"등록되지 않은 카드입니다: {unit.CardDefinitionId}");
            if (!State.Board.TryGetPosition(unit.Id, out int x, out int y))
                throw new InvalidOperationException($"보드에 없는 유닛입니다: {unit.Id}");

            UnitView view = boardView.CreateUnitView(unitPrefab, x, y);
            view.Bind(unit, card);
            unitViews.Add(unit.Id, view);
        }

        private void OnCellClicked(int x, int y)
        {
            if (!isActiveAndEnabled)
                return;

            int playerId = State.CurrentPlayerId;
            int? occupant = State.Board.GetUnitIdAt(x, y);
            if (occupant.HasValue && State.Units[occupant.Value].OwnerId == playerId)
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
                AttackUnit(playerId, selectedUnitId.Value, occupant.Value);
            else
                MoveUnit(playerId, selectedUnitId.Value, x, y);
        }

        private void MoveUnit(int playerId, int unitId, int x, int y)
        {
            if (!engine.MoveUnit(playerId, unitId, x, y, out string reason))
            {
                status = reason;
                return;
            }

            // 엔진이 확정한 위치를 기준으로 UI를 옮긴다.
            if (State.Board.TryGetPosition(unitId, out int newX, out int newY))
            {
                UnitView view = unitViews[unitId];
                boardView.MoveUnitView(view, newX, newY);
                view.Refresh(State.Units[unitId]);
            }

            status = State.Players[playerId].ActionResource == 0
                ? "행동을 모두 사용했습니다. 턴 종료 버튼을 눌러주세요."
                : "이동했습니다. 같은 유닛으로 한 번 더 행동할 수 있습니다.";
            RefreshHighlights();
        }

        private void AttackUnit(int playerId, int attackerId, int defenderId)
        {
            if (!engine.AttackUnit(playerId, attackerId, defenderId,
                    out CombatResult result, out string reason))
            {
                status = reason;
                return;
            }

            if (unitViews.TryGetValue(defenderId, out UnitView view))
            {
                if (result.DefenderDied)
                {
                    view.gameObject.SetActive(false);
                    Destroy(view.gameObject);
                    unitViews.Remove(defenderId);
                }
                else
                {
                    view.Refresh(State.Units[defenderId]);
                }
            }

            status = result.DefenderDied
                ? $"{result.Damage} 피해. 적을 처치했습니다."
                : $"{result.Damage} 피해. 상대 HP: {result.RemainingHP}";
            if (State.Players[playerId].ActionResource == 0)
                status += " 턴 종료 버튼을 눌러주세요.";
            RefreshHighlights();
        }

        private void EndTurn()
        {
            // 양쪽을 번갈아 조작하는 화면이므로 현재 플레이어로 요청한다.
            if (!engine.EndTurn(State.CurrentPlayerId, out string reason))
            {
                status = reason;
                return;
            }

            selectedUnitId = null;
            boardView.ClearHighlights();
            status = $"플레이어 {State.CurrentPlayerId}의 턴입니다.";
        }

        private void RefreshHighlights()
        {
            boardView.ClearHighlights();
            if (!selectedUnitId.HasValue)
                return;

            int unitId = selectedUnitId.Value;
            for (int y = 0; y < State.Board.Height; y++)
            {
                for (int x = 0; x < State.Board.Width; x++)
                {
                    if (engine.CanMoveUnit(State.CurrentPlayerId, unitId, x, y, out _))
                        boardView.HighlightCell(x, y, CellHighlight.Move);

                    int? targetId = State.Board.GetUnitIdAt(x, y);
                    if (targetId.HasValue && engine.CanAttackUnit(State.CurrentPlayerId, unitId, targetId.Value, out _))
                        boardView.HighlightCell(x, y, CellHighlight.Attack);
                }
            }

            if (State.Board.TryGetPosition(unitId, out int unitX, out int unitY))
                boardView.HighlightCell(unitX, unitY, CellHighlight.Selected);
        }

        // 임시 HUD. 별도 HUD View를 만들면 옮긴다.
        private void OnGUI()
        {
            if (engine == null)
                return;

            GUILayout.BeginArea(new Rect(12, 12, Mathf.Min(520, Screen.width - 24), 150), GUI.skin.box);
            GUILayout.Label($"{State.TurnNumber}턴 · 플레이어 {State.CurrentPlayerId}");
            GUILayout.Label($"남은 행동: {State.Players[State.CurrentPlayerId].ActionResource} / {GameEngine.ActionsPerTurn}");
            GUILayout.Label(status);
            if (GUILayout.Button("턴 종료"))
                EndTurn();
            GUILayout.EndArea();
        }
    }
}
