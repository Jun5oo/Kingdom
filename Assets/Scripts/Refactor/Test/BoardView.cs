using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Refactor
{
    // 보드의 표시만 담당한다. 유닛 상태나 이동 규칙은 변경하지 않는다.
    [RequireComponent(typeof(RectTransform))]
    public sealed class BoardView : MonoBehaviour
    {
        [SerializeField] private Color lightColor = new Color(0.82f, 0.85f, 0.79f);
        [SerializeField] private Color darkColor = new Color(0.25f, 0.34f, 0.32f);
        [SerializeField] private Color moveHighlightColor = new Color(0.35f, 0.65f, 0.45f);
        [SerializeField] private Color attackHighlightColor = new Color(0.85f, 0.35f, 0.35f);
        [SerializeField] private Color selectedHighlightColor = new Color(0.9f, 0.7f, 0.25f);
        [SerializeField, Min(0)] private float gap = 2f;
        [SerializeField, HideInInspector] private int width;
        [SerializeField, HideInInspector] private int height;
        [SerializeField, HideInInspector] private List<RectTransform> cells = new();

        public int Width => width;
        public int Height => height;
        public event Action<int, int> CellClicked;

        // 초기화 코드에서 engine.State.Board를 전달하면 된다.
        public void Show(BoardState board)
        {
            if (board == null)
                throw new ArgumentNullException(nameof(board));

            ClearCells();
            width = board.Width;
            height = board.Height;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    var cell = new GameObject($"Cell ({x}, {y})", typeof(RectTransform), typeof(Image), typeof(Button));
                    var rect = (RectTransform)cell.transform;
                    rect.SetParent(transform, false);
                    // (0, 0)은 왼쪽 아래. 보드가 리사이즈되어도 칸 비율을 유지한다.
                    rect.anchorMin = new Vector2((float)x / width, (float)y / height);
                    rect.anchorMax = new Vector2((float)(x + 1) / width, (float)(y + 1) / height);
                    rect.offsetMin = Vector2.one * gap * 0.5f;
                    rect.offsetMax = -Vector2.one * gap * 0.5f;

                    Image image = cell.GetComponent<Image>();
                    image.color = (x + y) % 2 == 0 ? lightColor : darkColor;
                    image.raycastTarget = true;
                    Button button = cell.GetComponent<Button>();
                    button.targetGraphic = image;
                    button.transition = Selectable.Transition.None;
                    button.navigation = new Navigation { mode = Navigation.Mode.None };
                    int cellX = x;
                    int cellY = y;
                    button.onClick.AddListener(() => CellClicked?.Invoke(cellX, cellY));
                    cells.Add(rect);
                }
            }
        }

        public void ClearHighlights()
        {
            for (int y = 0; y < height; y++)
                for (int x = 0; x < width; x++)
                    SetCellColor(x, y, (x + y) % 2 == 0 ? lightColor : darkColor);
        }

        public void HighlightCell(int x, int y, CellHighlight highlight)
        {
            Color color = highlight switch
            {
                CellHighlight.Move => moveHighlightColor,
                CellHighlight.Attack => attackHighlightColor,
                CellHighlight.Selected => selectedHighlightColor,
                _ => throw new ArgumentOutOfRangeException(nameof(highlight))
            };
            SetCellColor(x, y, color);
        }

        public UnitView CreateUnitView(UnitView prefab, int x, int y)
        {
            UnitView view = Instantiate(prefab, GetCell(x, y), false);
            RectTransform rect = (RectTransform)view.transform;
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = new Vector2(4, 4);
            rect.offsetMax = new Vector2(-4, -4);
            rect.localScale = Vector3.one;
            return view;
        }

        public void MoveUnitView(UnitView view, int x, int y)
        {
            view.transform.SetParent(GetCell(x, y), false);
        }

        public void SetCellColor(int x, int y, Color color)
        {
            GetCell(x, y).GetComponent<Image>().color = color;
        }

        // 이후 유닛 UI를 해당 칸의 중심에 표시할 때 사용한다.
        public RectTransform GetCell(int x, int y)
        {
            if (x < 0 || x >= width)
                throw new ArgumentOutOfRangeException(nameof(x));
            if (y < 0 || y >= height)
                throw new ArgumentOutOfRangeException(nameof(y));

            return cells[y * width + x];
        }

        private void ClearCells()
        {
            // 직접 생성한 칸만 제거한다. 다른 자식 객체는 건드리지 않는다.
            foreach (RectTransform cell in cells)
            {
                if (cell == null)
                    continue;

                cell.gameObject.SetActive(false);
                if (Application.isPlaying)
                    Destroy(cell.gameObject);
                else
                    DestroyImmediate(cell.gameObject);
            }

            cells.Clear();
        }
    }
}
