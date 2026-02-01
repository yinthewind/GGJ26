using System;
using System.Collections.Generic;
using UnityEngine;

public class WorkspaceControllers
{
    public static WorkspaceControllers Instance { get; } = new();

    private readonly List<WorkspaceController> _workspaceControllers = new();

    public IReadOnlyList<WorkspaceController> Workspaces => _workspaceControllers;

    public void Add(WorkspaceController controller)
    {
        _workspaceControllers.Add(controller);
    }

    public void Remove(WorkspaceController controller)
    {
        _workspaceControllers.Remove(controller);
    }

    public void Clear()
    {
        _workspaceControllers.Clear();
    }

    public WorkspaceController GetByEntityId(Guid entityId)
    {
        foreach (var controller in _workspaceControllers)
        {
            if (controller.EntityId == entityId)
                return controller;
        }
        return null;
    }

    public WorkspaceController FindAtPosition(Vector2 pos)
    {
        foreach (var controller in _workspaceControllers)
        {
            if (controller.ContainsPoint(pos))
                return controller;
        }
        return null;
    }

    public WorkspaceController GetWorkspaceAtGridPosition(Vector2Int gridPos)
    {
        foreach (var controller in _workspaceControllers)
        {
            foreach (var cell in controller.GetOccupiedGridCells())
            {
                if (cell == gridPos)
                    return controller;
            }
        }
        return null;
    }

    public bool IsGridPositionOccupied(Vector2Int gridPos, Guid? excludeEntityId = null)
    {
        foreach (var controller in _workspaceControllers)
        {
            if (excludeEntityId.HasValue && controller.EntityId == excludeEntityId.Value)
                continue;

            foreach (var cell in controller.GetOccupiedGridCells())
            {
                if (cell == gridPos)
                    return true;
            }
        }
        return false;
    }

    public bool IsValidPlacement(Vector2Int gridPos, Vector2Int gridSize, Guid? excludeEntityId = null)
    {
        // Must be a valid workspace position in the diamond grid
        if (!GridSystem.IsValidWorkspacePosition(gridPos))
            return false;

        // Check all cells the workspace would occupy at target position
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                var cellPos = new Vector2Int(gridPos.x + x, gridPos.y + y);

                // Can't place where another workspace exists
                if (IsGridPositionOccupied(cellPos, excludeEntityId))
                    return false;
            }
        }

        // Must be orthogonally adjacent to at least one existing workspace
        return IsAdjacentToExistingWorkspace(gridPos, gridSize, excludeEntityId);
    }

    private bool IsAdjacentToExistingWorkspace(Vector2Int gridPos, Vector2Int gridSize, Guid? excludeEntityId = null)
    {
        // Check all cells the new workspace would occupy
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                var cellPos = new Vector2Int(gridPos.x + x, gridPos.y + y);

                // Check 4 orthogonal neighbors of this cell
                var adjacentPositions = GridSystem.GetAdjacentPositions(cellPos);
                foreach (var adjPos in adjacentPositions)
                {
                    if (IsGridPositionOccupied(adjPos, excludeEntityId))
                        return true;  // Found an adjacent workspace
                }
            }
        }
        return false;  // No adjacent workspace found
    }

    public WorkspaceController SpawnWorkspace(Vector3 position, Vector2Int gridSize, Vector2Int gridPosition, WorkspaceType type, string id = "Unknown")
    {
        var go = WorkspaceAnimator.Create(position, gridSize, id);
        var animator = go.GetComponent<WorkspaceAnimator>();
        var controller = new WorkspaceController(go.transform, gridSize, gridPosition, type, animator);
        WorkspaceAnimator.Register(controller.EntityId, animator);
        Add(controller);

        return controller;
    }

    public WorkspaceController SpawnWorkspaceWithId(Guid entityId, Vector3 position, Vector2Int gridSize, Vector2Int gridPosition, WorkspaceType type, string id = "Unknown")
    {
        var go = WorkspaceAnimator.Create(position, gridSize, id);
        var animator = go.GetComponent<WorkspaceAnimator>();
        var controller = new WorkspaceController(entityId, go.transform, gridSize, gridPosition, type, animator);
        WorkspaceAnimator.Register(controller.EntityId, animator);
        Add(controller);

        return controller;
    }
}
