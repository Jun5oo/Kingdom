using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Refactor
{
    // 씬 시작 시 게임 객체를 생성하고 연결한다.
    public sealed class GameInitializer : MonoBehaviour
    {
        private const int FirstPlayerId = 1;
        private const int SecondPlayerId = 2;

        [SerializeField] private BoardView boardView;
        [SerializeField] private UnitView unitPrefab;
        [SerializeField] private GamePresenter presenter;
        [SerializeField] private List<PlacementEntry> initialUnits = new();

        // 규칙 코드의 UnitPlacement는 Unity 직렬화를 쓰지 않으므로, 인스펙터 입력용 구조체를 따로 둔다.
        [Serializable]
        private struct PlacementEntry
        {
            [SerializeField] private int ownerId;
            [SerializeField] private string cardId;
            [SerializeField, Min(1)] private int level;
            [SerializeField] private Vector2Int position;

            public UnitPlacement ToPlacement() =>
                new(ownerId, cardId, level, new GridPos(position.x, position.y));
        }

        private void Start()
        {
            if (!IsSceneConfigured())
                return;

            if (!CardFiles.TryCreateDatabase(this, out CardDatabase database))
                return;

            GameState state = new(new PlayerState(FirstPlayerId), new PlayerState(SecondPlayerId));

            GameEngine engine = new(state, database);
            List<UnitPlacement> placements = initialUnits.ConvertAll(entry => entry.ToPlacement());
            if (!engine.StartGame(placements, out string reason))
            {
                Debug.LogError(reason, this);
                return;
            }

            presenter.Initialize(engine, database, boardView, unitPrefab);
        }

        private bool IsSceneConfigured()
        {
            if (boardView == null || unitPrefab == null || presenter == null)
            {
                Debug.LogError("보드, 유닛 프리팹, Presenter를 지정해주세요.", this);
                return false;
            }

            if (!unitPrefab.IsConfigured)
            {
                Debug.LogError("유닛 프리팹의 Artwork, Name Text, CP Text를 연결해주세요.", this);
                return false;
            }

            Canvas canvas = boardView.GetComponentInParent<Canvas>();
            if (canvas == null || EventSystem.current == null)
            {
                Debug.LogError("보드의 Canvas와 씬의 EventSystem이 필요합니다.", this);
                return false;
            }

            if (canvas.GetComponent<GraphicRaycaster>() == null)
                canvas.gameObject.AddComponent<GraphicRaycaster>();

            return true;
        }
    }
}
