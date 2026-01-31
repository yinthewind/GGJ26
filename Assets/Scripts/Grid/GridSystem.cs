using System.Collections.Generic;
using UnityEngine;

public static class GridSystem
{
    public const float CellSize = 1.0f;

    // Valid workspace positions form a diamond (45° rotated square)
    private static readonly HashSet<Vector2Int> ValidWorkspacePositions = new()
    {
        new(0, 0), new(2, 0), new(-2, 0), new(0, 2), new(0, -2),
        new(1, 1), new(-1, 1), new(1, -1), new(-1, -1)
    };

    public static bool IsValidWorkspacePosition(Vector2Int gridPosition)
    {
        return ValidWorkspacePositions.Contains(gridPosition);
    }

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
        // Diagonal neighbors for the rotated diamond grid
        return new Vector2Int[]
        {
            gridPosition + new Vector2Int(-1, 1),   // upper-left
            gridPosition + new Vector2Int(1, 1),    // upper-right
            gridPosition + new Vector2Int(-1, -1),  // lower-left
            gridPosition + new Vector2Int(1, -1)    // lower-right
        };
    }
}
