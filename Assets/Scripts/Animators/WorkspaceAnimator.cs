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
    private static readonly Color OutlineColor = Color.black;
    private const float OutlineWidth = 0.05f;

    private Vector2Int _gridSize;
    private SpriteRenderer _spriteRenderer;
    private LineRenderer _outlineRenderer;

    public static GameObject Create(Vector3 position, Vector2Int gridSize, string id = "Unknown")
    {
        var go = new GameObject($"{Tag}:{id}");
        go.transform.position = position;

        var animator = go.AddComponent<WorkspaceAnimator>();
        animator._gridSize = gridSize;
        animator.CreateVisual();
        animator.CreateOutline();

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

    private void CreateOutline()
    {
        var outlineGo = new GameObject("Outline");
        outlineGo.transform.SetParent(transform);
        outlineGo.transform.localPosition = Vector3.zero;
        outlineGo.transform.localRotation = Quaternion.Euler(0, 0, 45f);

        _outlineRenderer = outlineGo.AddComponent<LineRenderer>();
        _outlineRenderer.useWorldSpace = false;
        _outlineRenderer.loop = true;
        _outlineRenderer.positionCount = 4;

        // Outline corners (in local space, accounting for center-pivot visual offset)
        float left = -0.5f;
        float right = _gridSize.x - 0.5f;
        float bottom = -0.5f;
        float top = _gridSize.y - 0.5f;

        _outlineRenderer.SetPosition(0, new Vector3(left, bottom, 0f));
        _outlineRenderer.SetPosition(1, new Vector3(right, bottom, 0f));
        _outlineRenderer.SetPosition(2, new Vector3(right, top, 0f));
        _outlineRenderer.SetPosition(3, new Vector3(left, top, 0f));

        _outlineRenderer.startWidth = OutlineWidth;
        _outlineRenderer.endWidth = OutlineWidth;
        _outlineRenderer.startColor = OutlineColor;
        _outlineRenderer.endColor = OutlineColor;
        _outlineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        _outlineRenderer.sortingOrder = 0;
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
        if (_outlineRenderer != null)
        {
            _outlineRenderer.enabled = visible;
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
        if (_outlineRenderer != null)
        {
            var color = _outlineRenderer.startColor;
            color.a = alpha;
            _outlineRenderer.startColor = color;
            _outlineRenderer.endColor = color;
        }
    }
}
