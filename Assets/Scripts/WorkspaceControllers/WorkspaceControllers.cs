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

    public WorkspaceController GetByEntityId(int entityId)
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

    public bool IsGridPositionOccupied(Vector2Int gridPos, int? excludeEntityId = null)
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

    public bool IsValidPlacement(Vector2Int gridPos, Vector2Int gridSize, int? excludeEntityId = null)
    {
        // Check all cells the workspace would occupy at target position
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                var cellPos = new Vector2Int(gridPos.x + x, gridPos.y + y);

                // Can't place where another workspace exists
                if (IsGridPositionOccupied(cellPos, excludeEntityId))
                    return false;

                // Can't place below ground (y < 1)
                if (cellPos.y < 1)
                    return false;
            }
        }

        // If moving an existing workspace, check if it can leave its current position
        // NOT OK if at least 1 cell directly above current position is occupied
        if (excludeEntityId.HasValue)
        {
            var currentWorkspace = GetByEntityId(excludeEntityId.Value);
            if (currentWorkspace != null)
            {
                var currentGridPos = currentWorkspace.GridPosition;
                var currentGridSize = currentWorkspace.GridSize;
                var currentTopY = currentGridPos.y + currentGridSize.y - 1;

                for (int x = 0; x < currentGridSize.x; x++)
                {
                    var abovePos = new Vector2Int(currentGridPos.x + x, currentTopY + 1);
                    if (IsGridPositionOccupied(abovePos, excludeEntityId))
                        return false;
                }
            }
        }

        // Target position: OK if at ground level (y == 1)
        if (gridPos.y == 1)
            return true;

        // Target position: OK if at least one cell directly below is occupied
        for (int x = 0; x < gridSize.x; x++)
        {
            var belowPos = new Vector2Int(gridPos.x + x, gridPos.y - 1);
            if (IsGridPositionOccupied(belowPos, excludeEntityId))
                return true;
        }

        // No support below and not at ground level
        return false;
    }

    public WorkspaceController SpawnWorkspace(Vector3 position, Vector2Int gridSize, WorkspaceType type, string id = "Unknown")
    {
        var go = WorkspaceAnimator.Create(position, gridSize, id);
        var animator = go.GetComponent<WorkspaceAnimator>();
        var controller = new WorkspaceController(go.transform, gridSize, type, animator);
        WorkspaceAnimator.Register(controller.EntityId, animator);
        Add(controller);

        return controller;
    }
}
