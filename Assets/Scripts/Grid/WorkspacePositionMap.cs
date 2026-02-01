using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Maps workspace grid coordinates to pixel positions for desk animator placement.
/// </summary>
public static class WorkspacePositionMap
{
    /// <summary>
    /// Map from grid coordinates to pixel positions.
    /// Grid coordinates follow the diamond pattern: (0,0) center,
    /// (1,1), (-1,1), (1,-1), (-1,-1) inner ring,
    /// (2,0), (-2,0), (0,2), (0,-2) outer ring.
    /// </summary>
    public static readonly Dictionary<Vector2Int, Vector3> pixelPositions = new ()
    {
        { new Vector2Int(-2, 0), new Vector3(431, 1080 - 587,0)},
        { new Vector2Int(-1, 1), new Vector3(618, 1080 - 493.5f,0)},
        { new Vector2Int(0, 2), new Vector3(802,1080 - 401.5f,0)},
        { new Vector2Int(-1, -1), new Vector3(617,1080 - 679,0)},
        { new Vector2Int(0, 0), new Vector3(801,1080 - 586,0)},
        { new Vector2Int(1, 1), new Vector3(987,1080 - 493.5f,0)},
        { new Vector2Int(0, -2), new Vector3(801.5f,1080 - 771.5f,0)},
        { new Vector2Int(1, -1), new Vector3(985,1080 - 678.5f,0)},
        { new Vector2Int(2, 0), new Vector3(1169,1080 - 585.5f,0)},
    };
    
    public static Vector3 DeskPivotOffset = new Vector3(53.5f, -20f);
    public static Vector3 WorkhorsePivotOffset = new Vector3(-13.4f, 65.56f);

    /// <summary>
    /// Get world position for a workspace (desk/chair) at a grid coordinate.
    /// Applies DeskPivotOffset and scales to world units.
    /// </summary>
    public static Vector3 GetWorldPosition(Vector2Int gridPos)
    {
        if (pixelPositions.TryGetValue(gridPos, out var pixelPos))
        {
            return (pixelPos + DeskPivotOffset) / 100;
        }
        return Vector3.zero;
    }

    /// <summary>
    /// Get world position for a workhorse at a grid coordinate.
    /// </summary>
    public static Vector3 GetWorkhorsePosition(Vector2Int gridPos)
    {
        if (pixelPositions.TryGetValue(gridPos, out var pixelPos))
        {
            return (pixelPos + WorkhorsePivotOffset) / 100;
        }
        return Vector3.zero;
    }

    /// <summary>
    /// Check if a grid position is valid.
    /// </summary>
    public static bool IsValidPosition(Vector2Int gridPos)
    {
        return pixelPositions.ContainsKey(gridPos);
    }
}
