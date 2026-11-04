using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

namespace Refactor.Editors
{
    public static class BoardViewMenu
    {
        [MenuItem("GameObject/Kingdom/Refactor Board (7x7)", false, 10)]
        private static void CreateBoard()
        {
            var canvasObject = new GameObject("Board Canvas",
                typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler));
            Canvas canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;

            var boardObject = new GameObject("Board", typeof(RectTransform), typeof(BoardView));
            var rect = (RectTransform)boardObject.transform;
            rect.SetParent(canvasObject.transform, false);
            rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(840, 840);
            rect.anchoredPosition = Vector2.zero;

            boardObject.GetComponent<BoardView>().Show(new BoardState(7, 7));
            Undo.RegisterCreatedObjectUndo(canvasObject, "Create Refactor Board");
            EditorSceneManager.MarkSceneDirty(canvasObject.scene);
            Selection.activeGameObject = boardObject;
        }

        [MenuItem("GameObject/Kingdom/Board", true)]
        private static bool CanCreateBoard() => !EditorApplication.isPlayingOrWillChangePlaymode;
    }
}
