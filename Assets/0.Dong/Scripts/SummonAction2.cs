using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SummonAction2 : MonoBehaviour
{
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private GridSystem gridSystem;

    private GameObject previewCard;
    private bool isSummoning = false;

    private Button summonButton;

    void Awake()
    {
        summonButton = GetComponent<Button>();
    }

    void Update()
    {
        if (!isSummoning) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.transform.TryGetComponent<GridCell>(out GridCell gridCell))
            {
                if (!gridCell.isHighlighted) return;

                previewCard.transform.position = gridCell.transform.position;

                if (Input.GetMouseButtonDown(0))
                {
                    Vector3 targetPos = gridCell.transform.position;
                    Quaternion targetRot = Quaternion.Euler(0f, 0f, -180f);
                    Vector3 targetScale = Vector3.one;

                    previewCard.GetComponent<CardMovement>().MoveTransform(
                        new PRS(targetPos, targetRot, targetScale),
                        0.3f
                    );

                    gridSystem.PlaceObjectTo(previewCard, gridCell.GetGridPosition());
                    gridSystem.UnhighlightGridCells();

                    isSummoning = false;
                    previewCard = null;
                }
            }
        }
    }

    public void StartSummon()
    {
        GameObject king = GameObject.FindWithTag("King");
        if (king == null)
        {
            Debug.LogWarning("왕이 배치되지 않았습니다!");
            return;
        }

        Vector2Int kingPos = gridSystem.GetGridPosition(king.transform.position);
        List<Vector2Int> validPositions = GetAdjacentPositions(kingPos);

        isSummoning = true;
        previewCard = Instantiate(cardPrefab);
        previewCard.transform.localScale = Vector3.one;

        gridSystem.HighlightGridCells((pos) => validPositions.Contains(pos));
        //summonButton.enabled = false;
    }

    private List<Vector2Int> GetAdjacentPositions(Vector2Int center)
    {
        List<Vector2Int> positions = new List<Vector2Int>();
        Vector2Int[] deltas = {
        Vector2Int.up,
        Vector2Int.down,
        Vector2Int.left,
        Vector2Int.right,
        new Vector2Int(-1, 1),  // ↖
        new Vector2Int(1, 1),   // ↗
        new Vector2Int(-1, -1), // ↙
        new Vector2Int(1, -1)   // ↘
    };

        foreach (var delta in deltas)
        {
            Vector2Int checkPos = center + delta;
            if (gridSystem.IsValidPosition(checkPos))
                positions.Add(checkPos);
        }

        return positions;
    }
}
