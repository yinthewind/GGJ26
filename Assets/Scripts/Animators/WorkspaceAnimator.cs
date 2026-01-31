using System;
using System.Collections.Generic;
using UnityEngine;

public class WorkspaceAnimator : MonoBehaviour
{
    public static string Tag = "WorkspaceAnimator";

    // Static registry
    private static readonly Dictionary<Guid, WorkspaceAnimator> _animators = new();

    public static void Register(Guid entityId, WorkspaceAnimator animator) => _animators[entityId] = animator;
    public static void Unregister(Guid entityId) => _animators.Remove(entityId);
    public static WorkspaceAnimator GetAnimator(Guid entityId) => _animators.TryGetValue(entityId, out var animator) ? animator : null;
    public static void ClearRegistry() => _animators.Clear();

    private static readonly Color DragTintColor = new Color(0.5f, 0.7f, 1f, 1f);

    private Vector2Int _gridSize;
    private SpriteRenderer _spriteRenderer;

    public static GameObject Create(Vector3 position, Vector2Int gridSize, string id = "Unknown")
    {
        var go = new GameObject($"{Tag}:{id}");
        go.transform.position = position;

        var animator = go.AddComponent<WorkspaceAnimator>();
        animator._gridSize = gridSize;
        animator.CreateVisual();

        return go;
    }

    private void CreateVisual()
    {
        var visualGo = new GameObject("Visual");
        visualGo.transform.SetParent(transform);

        // Offset visual to center over the full footprint (anchor is bottom-left)
        var offsetX = (_gridSize.x - 1) * 0.5f;
        var offsetY = (_gridSize.y - 1) * 0.5f;
        visualGo.transform.localPosition = new Vector3(offsetX, offsetY, 0f);
        visualGo.transform.localScale = new Vector3(_gridSize.x, _gridSize.y, 1f);

        _spriteRenderer = visualGo.AddComponent<SpriteRenderer>();
        _spriteRenderer.sprite = SpriteLoader.Instance.GetSprite("Sprites/desk");
        _spriteRenderer.color = Color.white;
        _spriteRenderer.sortingOrder = -1;
    }

    public void SetDragTint(bool isDragging)
    {
        if (_spriteRenderer != null)
        {
            _spriteRenderer.color = isDragging ? DragTintColor : Color.white;
        }
    }

    public void SetVisible(bool visible)
    {
        if (_spriteRenderer != null)
        {
            _spriteRenderer.enabled = visible;
        }
    }

    public void SetTransparent(bool transparent)
    {
        float alpha = transparent ? 0.3f : 1f;

        if (_spriteRenderer != null)
        {
            var color = _spriteRenderer.color;
            color.a = alpha;
            _spriteRenderer.color = color;
        }
    }
}
