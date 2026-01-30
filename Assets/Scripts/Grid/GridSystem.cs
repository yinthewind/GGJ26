using UnityEngine;

public static class GridSystem
{
    public const float CellSize = 1.0f;

    public static Vector3 SnapToGrid(Vector3 worldPosition)
    {
        var gridPos = WorldToGrid(worldPosition);
        return GridToWorld(gridPos);
    }

    public static Vector2Int WorldToGrid(Vector3 worldPosition)
    {
        return new Vector2Int(
            Mathf.RoundToInt(worldPosition.x / CellSize),
            Mathf.RoundToInt(worldPosition.y / CellSize)
        );
    }

    public static Vector3 GridToWorld(Vector2Int gridPosition)
    {
        return new Vector3(
            gridPosition.x * CellSize,
            gridPosition.y * CellSize,
            0f
        );
    }

    public static Vector2Int[] GetAdjacentPositions(Vector2Int gridPosition)
    {
        return new Vector2Int[]
        {
            gridPosition + Vector2Int.up,
            gridPosition + Vector2Int.down,
            gridPosition + Vector2Int.left,
            gridPosition + Vector2Int.right
        };
    }
}
