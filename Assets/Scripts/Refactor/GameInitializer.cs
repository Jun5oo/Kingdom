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
        [SerializeField] private List<UnitCardData> cards = new();
        [SerializeField] private List<UnitPlacement> initialUnits = new();

        private void Start()
        {
            if (!IsSceneConfigured())
                return;

            CardDatabase database;
            
            try
            {
                database = new CardDatabase(cards);
            }
            catch (ArgumentException exception)
            {
                Debug.LogError(exception.Message, this);
                return;
            }

            GameState state = new(new PlayerState(FirstPlayerId), new PlayerState(SecondPlayerId));
            
            GameEngine engine = new(state, database);
            if (!engine.StartGame(initialUnits, out string reason))
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
