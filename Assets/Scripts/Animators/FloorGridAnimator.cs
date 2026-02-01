using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Creates 9 individual floor sprites, one for each workspace position,
/// using separate sprites from the floors folder.
/// </summary>
public class FloorGridAnimator : MonoBehaviour
{
    public static string Tag = "FloorGridAnimator";

    private static readonly Dictionary<Vector2Int, string> FloorSpriteNames = new()
    {
        { new Vector2Int(-1, 1), "D1-2" },
        { new Vector2Int(0, 2), "D1-3" },
        { new Vector2Int(1, 1), "D2-3" },
        { new Vector2Int(-2, 0), "D1-1" },
        { new Vector2Int(0, 0), "D2-2" },
        { new Vector2Int(2, 0), "D3-3" },
        { new Vector2Int(-1, -1), "D2-1" },
        { new Vector2Int(0, -2), "D3-1" },
        { new Vector2Int(1, -1), "D3-2" },
    };

    public static GameObject Create()
    {
        var go = new GameObject(Tag);
        go.transform.position = Vector3.zero;

        var animator = go.AddComponent<FloorGridAnimator>();
        animator.CreateVisuals();

        return go;
    }

    private void CreateVisuals()
    {
        foreach (var (gridPos, spriteName) in FloorSpriteNames)
        {
            CreateFloorTile(gridPos, spriteName);
        }
    }

    private void CreateFloorTile(Vector2Int gridPos, string spriteName)
    {
        var tileGo = new GameObject($"Floor_{spriteName}");
        tileGo.transform.SetParent(transform);
        tileGo.transform.localPosition = WorkspacePositionMap.GetWorldPosition(gridPos);
        // tileGo.transform.localScale = Vector3.one * 0.5f;

        var sr = tileGo.AddComponent<SpriteRenderer>();
        sr.sprite = SpriteLoader.Instance.GetSprite($"Sprites/floors/{spriteName}");
        sr.color = Color.white;
        sr.sortingOrder = -2;

        // Add collider for raycast-based grid position detection
        var collider = tileGo.AddComponent<PolygonCollider2D>();

        // Store the grid position for lookups
        var floorInfo = tileGo.AddComponent<FloorTileInfo>();
        floorInfo.GridPosition = gridPos;

        // Register with the manager for highlight operations
        FloorGridAnimatorManager.Register(gridPos, sr);
    }
}
