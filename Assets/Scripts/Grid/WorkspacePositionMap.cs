using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Maps workspace grid coordinates to world positions for desk animator placement.
/// </summary>
public static class WorkspacePositionMap
{
    /// <summary>
    /// Map from grid coordinates to world positions.
    /// Grid coordinates follow the diamond pattern: (0,0) center,
    /// (1,1), (-1,1), (1,-1), (-1,-1) inner ring,
    /// (2,0), (-2,0), (0,2), (0,-2) outer ring.
    /// </summary>
    public static readonly Dictionary<Vector2Int, Vector3> Positions = new()
    {
        // Center
        { new Vector2Int(0, 0), new Vector3(0f, 0f, 0f) },

        // Inner ring (diagonal neighbors of center)
        { new Vector2Int(1, 1), new Vector3(1f, 1f, 0f) },
        { new Vector2Int(-1, 1), new Vector3(-1f, 1f, 0f) },
        { new Vector2Int(1, -1), new Vector3(1f, -1f, 0f) },
        { new Vector2Int(-1, -1), new Vector3(-1f, -1f, 0f) },

        // Outer ring (cardinal directions)
        { new Vector2Int(2, 0), new Vector3(2f, 0f, 0f) },
        { new Vector2Int(-2, 0), new Vector3(-2f, 0f, 0f) },
        { new Vector2Int(0, 2), new Vector3(0f, 2f, 0f) },
        { new Vector2Int(0, -2), new Vector3(0f, -2f, 0f) },
    };

    /// <summary>
    /// Get world position for a grid coordinate.
    /// </summary>
    public static Vector3 GetWorldPosition(Vector2Int gridPos)
    {
        return Positions.TryGetValue(gridPos, out var worldPos) ? worldPos : Vector3.zero;
    }

    /// <summary>
    /// Check if a grid position is valid.
    /// </summary>
    public static bool IsValidPosition(Vector2Int gridPos)
    {
        return Positions.ContainsKey(gridPos);
    }
}
