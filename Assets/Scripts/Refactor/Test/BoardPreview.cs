using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Refactor
{
    // 테스트용 초기화. 양쪽 플레이어의 유닛을 생성해 보드에 표시한다.
    public sealed class BoardPreview : MonoBehaviour
    {
        [SerializeField] private BoardView boardView;
        [SerializeField] private UnitView unitPrefab;

        [Header("테스트 유닛")]
        [SerializeField] private string cardId;
        [SerializeField, Min(1)] private int level = 1;
        [SerializeField] private Vector2Int spawnPosition = new Vector2Int(2, 0);

        [Header("상대 테스트 유닛")]
        [Tooltip("비워두면 위의 테스트 카드 ID를 사용합니다.")]
        [SerializeField] private string opponentCardId;
        [SerializeField, Min(1)] private int opponentLevel = 1;
        [SerializeField] private Vector2Int opponentSpawnPosition = new Vector2Int(4, 6);

        private GameState state;
        private CardDatabase database;
        private GameEngine engine;
        private readonly Dictionary<int, UnitView> unitViews = new();
        private int? selectedUnitId;
        private string status = "유닛을 선택해주세요.";

        private void OnEnable()
        {
            if (boardView != null)
                boardView.CellClicked += OnCellClicked;
        }

        private void OnDisable()
        {
            if (boardView != null)
                boardView.CellClicked -= OnCellClicked;
        }

        private void Start()
        {
            if (boardView == null || unitPrefab == null)
            {
                Debug.LogError("보드와 유닛 프리팹을 지정해주세요.", this);
                return;
            }

            if (!unitPrefab.IsConfigured)
            {
                Debug.LogError("유닛 프리팹의 Artwork, Name Text, CP Text를 연결해주세요.", this);
                return;
            }

            Canvas canvas = boardView.GetComponentInParent<Canvas>();
            if (canvas == null || EventSystem.current == null)
            {
                Debug.LogError("보드의 Canvas와 씬의 EventSystem이 필요합니다.", this);
                return;
            }
            if (canvas.GetComponent<GraphicRaycaster>() == null)
                canvas.gameObject.AddComponent<GraphicRaycaster>();

            if (!CardFiles.TryCreateDatabase(this, out database))
                return;

            string enemyCardId = string.IsNullOrWhiteSpace(opponentCardId) ? cardId : opponentCardId;
            UnitPlacement[] placements =
            {
                new UnitPlacement(1, cardId, level, new GridPos(spawnPosition.x, spawnPosition.y)),
                new UnitPlacement(2, enemyCardId, opponentLevel,
                    new GridPos(opponentSpawnPosition.x, opponentSpawnPosition.y))
            };

            state = new GameState(new PlayerState(1), new PlayerState(2));
            GameEngine startedEngine = new GameEngine(state, database);
            if (!startedEngine.StartGame(placements, out string startReason))
            {
                Debug.LogError(startReason, this);
                return;
            }

            engine = startedEngine;
            // Show는 기존 칸을 다시 만들므로 유닛 UI 생성 전에 한 번 호출한다.
            boardView.Show(state.Board);
            foreach (UnitState unit in state.Units.Values)
            {
                if (database.TryGetCard(unit.CardDefinitionId, out CardDefinition card))
                    ShowUnit(unit, card);
            }

            status = "플레이어 1의 유닛을 선택해주세요.";
        }

        private void ShowUnit(UnitState unit, CardDefinition card)
        {
            if (!state.Board.TryGetPosition(unit.Id, out int x, out int y))
                return;

            RectTransform cell = boardView.GetCell(x, y);
            UnitView view = Instantiate(unitPrefab, cell, false);
            var rect = (RectTransform)view.transform;
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = new Vector2(4, 4);
            rect.offsetMax = new Vector2(-4, -4);
            rect.localScale = Vector3.one;
            view.Bind(unit, card);
            unitViews.Add(unit.Id, view);
        }

        private void OnCellClicked(int x, int y)
        {
            if (engine == null || !isActiveAndEnabled)
                return;

            int playerId = state.CurrentPlayerId;
            int? occupant = state.Board.GetUnitIdAt(x, y);
            if (occupant.HasValue && state.Units[occupant.Value].OwnerId == playerId)
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

            int unitId = selectedUnitId.Value;
            if (occupant.HasValue)
            {
                AttackSelectedUnit(playerId, unitId, occupant.Value);
                return;
            }

            if (!engine.MoveUnit(playerId, unitId, x, y, out string reason))
            {
                status = reason;
                return;
            }

            // 엔진이 이동을 확정한 후, 실제 상태의 위치에 UI를 옮긴다.
            if (state.Board.TryGetPosition(unitId, out int newX, out int newY))
            {
                UnitView view = unitViews[unitId];
                view.transform.SetParent(boardView.GetCell(newX, newY), false);
                view.Refresh(state.Units[unitId]);
            }

            status = state.Players[playerId].ActionResource == 0
                ? "행동 2회를 모두 사용했습니다. 턴 종료 버튼을 눌러주세요."
                : "이동했습니다. 같은 유닛으로 한 번 더 행동할 수 있습니다.";
            RefreshHighlights();
        }

        private void AttackSelectedUnit(int playerId, int attackerId, int defenderId)
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
                    view.Refresh(state.Units[defenderId]);
                }
            }

            status = result.DefenderDied
                ? $"{result.Damage} 피해. 적을 처치했습니다."
                : $"{result.Damage} 피해. 상대 HP: {result.RemainingHP}";
            if (state.Players[playerId].ActionResource == 0)
                status += " 턴 종료 버튼을 눌러주세요.";
            RefreshHighlights();
        }

        public void OnEndTurnClicked()
        {
            if (engine == null || !isActiveAndEnabled)
                return;

            // 양쪽을 번갈아 조작하는 테스트 화면이므로 현재 플레이어로 요청한다.
            if (!engine.EndTurn(state.CurrentPlayerId, out string reason))
            {
                status = reason;
                return;
            }

            selectedUnitId = null;
            boardView.ClearHighlights();
            status = $"플레이어 {state.CurrentPlayerId}의 턴입니다.";
        }

        private void RefreshHighlights()
        {
            boardView.ClearHighlights();
            if (!selectedUnitId.HasValue)
                return;

            int unitId = selectedUnitId.Value;
            for (int y = 0; y < state.Board.Height; y++)
            {
                for (int x = 0; x < state.Board.Width; x++)
                {
                    if (engine.CanMoveUnit(state.CurrentPlayerId, unitId, x, y, out _))
                        boardView.SetCellColor(x, y, new Color(0.35f, 0.65f, 0.45f));
                    int? targetId = state.Board.GetUnitIdAt(x, y);
                    if (targetId.HasValue && engine.CanAttackUnit(state.CurrentPlayerId, unitId, targetId.Value, out _))
                        boardView.SetCellColor(x, y, new Color(0.85f, 0.35f, 0.35f));
                }
            }

            if (state.Board.TryGetPosition(unitId, out int unitX, out int unitY))
                boardView.SetCellColor(unitX, unitY, new Color(0.9f, 0.7f, 0.25f));
        }

        // 테스트용 표시. 별도의 텍스트 UI 연결 없이 상태를 확인한다.
        private void OnGUI()
        {
            if (engine == null)
                return;

            GUILayout.BeginArea(new Rect(12, 12, Mathf.Min(520, Screen.width - 24), 150), GUI.skin.box);
            GUILayout.Label($"{state.TurnNumber}턴 · 플레이어 {state.CurrentPlayerId}");
            GUILayout.Label($"남은 행동: {state.Players[state.CurrentPlayerId].ActionResource} / 2");
            GUILayout.Label(status);
            if (GUILayout.Button("턴 종료"))
                OnEndTurnClicked();
            GUILayout.EndArea();
        }
    }
}
