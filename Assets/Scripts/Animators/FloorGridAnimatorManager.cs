using System.Collections.Generic;
using UnityEngine;

public enum HighlightType
{
    None,
    Valid,
    Invalid,
    Hover
}

/// <summary>
/// Static manager that maps grid coordinates to floor tile SpriteRenderers,
/// enabling highlight operations at specific grid positions.
/// </summary>
public static class FloorGridAnimatorManager
{
    private static readonly Dictionary<Vector2Int, SpriteRenderer> _floorTiles = new();
    private static readonly HashSet<Vector2Int> _highlightedTiles = new();

    private static readonly Color ColorDefault = Color.white;
    private static readonly Color ColorValid = new Color(0.5f, 1f, 0.5f);
    private static readonly Color ColorInvalid = new Color(1f, 0.5f, 0.5f);
    private static readonly Color ColorHover = new Color(0.7f, 0.85f, 1f);

    public static void Register(Vector2Int gridPos, SpriteRenderer sr)
    {
        _floorTiles[gridPos] = sr;
    }

    public static void Unregister(Vector2Int gridPos)
    {
        _floorTiles.Remove(gridPos);
        _highlightedTiles.Remove(gridPos);
    }

    public static void ClearRegistry()
    {
        _floorTiles.Clear();
        _highlightedTiles.Clear();
    }

    public static void SetHighlight(Vector2Int gridPos, HighlightType type)
    {
        if (!_floorTiles.TryGetValue(gridPos, out var sr))
            return;

        sr.color = GetColorForHighlight(type);

        if (type == HighlightType.None)
            _highlightedTiles.Remove(gridPos);
        else
            _highlightedTiles.Add(gridPos);
    }

    public static void ClearHighlight(Vector2Int gridPos)
    {
        SetHighlight(gridPos, HighlightType.None);
    }

    public static void ClearAllHighlights()
    {
        foreach (var gridPos in _highlightedTiles)
        {
            if (_floorTiles.TryGetValue(gridPos, out var sr))
                sr.color = ColorDefault;
        }
        _highlightedTiles.Clear();
    }

    public static SpriteRenderer GetSpriteRenderer(Vector2Int gridPos)
    {
        _floorTiles.TryGetValue(gridPos, out var sr);
        return sr;
    }

    private static Color GetColorForHighlight(HighlightType type)
    {
        return type switch
        {
            HighlightType.Valid => ColorValid,
            HighlightType.Invalid => ColorInvalid,
            HighlightType.Hover => ColorHover,
            _ => ColorDefault
        };
    }
}
