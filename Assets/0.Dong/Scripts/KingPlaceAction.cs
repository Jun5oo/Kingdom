using UnityEngine;
using UnityEngine.UI;
public class KingPlaceAction : MonoBehaviour
{
    [SerializeField] private GameObject kingPrefab;
    [SerializeField] private GridSystem gridSystem;

    Button kingButton;
    private GameObject previewKing;
    private bool isPlacing = false;
    private void Awake()
    {
        kingButton = GetComponent<Button>();
    }
    void Update()
    {
        if (!isPlacing) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.transform.TryGetComponent<GridCell>(out GridCell gridCell))
            {
                if (!gridCell.isHighlighted) return;

                previewKing.transform.position = gridCell.transform.position;

                if (Input.GetMouseButtonDown(0))
                {
                    Vector3 targetPos = gridCell.transform.position;
                    Quaternion targetRot = Quaternion.Euler(0f, 0f, -180f); // 왕 정면 방향
                    Vector3 targetScale = Vector3.one;

                    previewKing.GetComponent<CardMovement>().MoveTransform(
                        new PRS(targetPos, targetRot, targetScale),
                        0.3f // 부드럽게 이동
                    );

                    gridSystem.PlaceObjectTo(previewKing, gridCell.GetGridPosition());
                    gridSystem.UnhighlightGridCells();

                    isPlacing = false;
                    previewKing = null;
                }
            }
        }
    }

    public void StartPlacement()
    {
        isPlacing = true;
        previewKing = Instantiate(kingPrefab);
        previewKing.transform.localScale = Vector3.one;

        gridSystem.HighlightGridCells((pos) => IsValidKingPosition(pos));
        kingButton.enabled = false;
    }

    private bool IsValidKingPosition(Vector2Int pos)
    {
        return pos.y < 3;
    }
}
