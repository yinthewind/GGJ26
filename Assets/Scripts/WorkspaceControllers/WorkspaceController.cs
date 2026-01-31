using System;
using System.Collections.Generic;
using UnityEngine;

public class WorkspaceController
{
    private readonly Guid _entityId;
    private readonly Transform _transform;
    private readonly Vector2Int _gridSize;
    private readonly Vector2Int _gridPosition;
    private readonly WorkspaceType _type;
    private readonly WorkspaceAnimator _animator;

    private Guid? _assignedSkeletonId;
    private bool _isDragging;
    private Vector3 _originalPosition;

    public WorkspaceController(Transform transform, Vector2Int gridSize, Vector2Int gridPosition, WorkspaceType type, WorkspaceAnimator animator)
        : this(Guid.NewGuid(), transform, gridSize, gridPosition, type, animator)
    {
    }

    public WorkspaceController(Guid entityId, Transform transform, Vector2Int gridSize, Vector2Int gridPosition, WorkspaceType type, WorkspaceAnimator animator)
    {
        _entityId = entityId;
        _transform = transform;
        _gridSize = gridSize;
        _gridPosition = gridPosition;
        _type = type;
        _animator = animator;
    }

    public Guid EntityId => _entityId;
    public WorkspaceAnimator Animator => _animator;
    public Vector3 Position => _transform.position;
    public Guid? AssignedSkeletonId => _assignedSkeletonId;
    public bool IsOccupied => _assignedSkeletonId.HasValue;
    public bool IsDragging => _isDragging;
    public Vector2Int GridPosition => _gridPosition;
    public Vector2Int GridSize => _gridSize;
    public WorkspaceType Type => _type;

    /// <summary>
    /// Returns the world center of the workspace (for skeleton positioning).
    /// For multi-cell workspaces, this is offset from the bottom-left anchor.
    /// </summary>
    public Vector3 WorldCenter
    {
        get
        {
            var offset = new Vector3((_gridSize.x - 1) * 0.5f, (_gridSize.y - 1) * 0.5f, 0f);
            return _transform.position + offset;
        }
    }

    public void AssignSkeleton(Guid skeletonEntityId)
    {
        _assignedSkeletonId = skeletonEntityId;
    }

    public void UnassignSkeleton()
    {
        _assignedSkeletonId = null;
    }

    /// <summary>
    /// Returns all grid cells occupied by this workspace.
    /// GridPosition is the bottom-left corner (anchor).
    /// </summary>
    public IEnumerable<Vector2Int> GetOccupiedGridCells()
    {
        var anchor = GridPosition;
        for (int x = 0; x < _gridSize.x; x++)
        {
            for (int y = 0; y < _gridSize.y; y++)
            {
                yield return new Vector2Int(anchor.x + x, anchor.y + y);
            }
        }
    }

    public bool ContainsPoint(Vector2 point)
    {
        var pos = _transform.position;
        var minX = pos.x - 0.5f;
        var maxX = pos.x + _gridSize.x - 0.5f;
        var minY = pos.y - 0.5f;
        var maxY = pos.y + _gridSize.y - 0.5f;

        return point.x >= minX && point.x <= maxX &&
               point.y >= minY && point.y <= maxY;
    }

    public void SetPosition(Vector3 position)
    {
        _transform.position = position;
    }

    public void OnDragStart()
    {
        _originalPosition = _transform.position;
        _isDragging = true;
    }

    public void OnDragEnd()
    {
        _isDragging = false;
    }

    public void RevertToOriginalPosition()
    {
        _transform.position = _originalPosition;
    }
}
