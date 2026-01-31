using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class CharacterControllers
{
    public static CharacterControllers Instance { get; } = new();

    private readonly List<WorkhorseController> _skeletonControllers = new();

    // Event fired when a worker is spawned
    // Parameters: entityId, workerType
    public event Action<int, WorkhorseType> OnMonsterSpawned;

    public IReadOnlyList<WorkhorseController> Skeletons => _skeletonControllers;

    public void Add(WorkhorseController controller)
    {
        _skeletonControllers.Add(controller);
    }

    public void Remove(WorkhorseController controller)
    {
        _skeletonControllers.Remove(controller);
    }

    public void Update(float deltaTime)
    {
        foreach (var controller in _skeletonControllers)
        {
            controller.Update(deltaTime);
        }
    }

    public void Clear()
    {
        _skeletonControllers.Clear();
    }

    public WorkhorseController GetByEntityId(int entityId)
    {
        foreach (var controller in _skeletonControllers)
        {
            if (controller.EntityId == entityId)
                return controller;
        }
        return null;
    }

    /// <summary>
    /// Spawns a new worker at the specified position.
    /// Creates the visual, controller, registers with all managers.
    /// </summary>
    public WorkhorseController SpawnSkeleton(WorkhorseType type, Vector3 position)
    {
        string prefabPath = WorkhorsePrefabs.GetPrefabPath(type);
        GameObject go = WorkhorseAnimator.Create(position, Quaternion.identity, prefabPath, type.ToString());
        WorkhorseAnimator animator = go.GetComponent<WorkhorseAnimator>();

        WorkhorseController controller = new WorkhorseController(go.transform, type, animator);
        WorkhorseAnimator.Register(controller.EntityId, animator);
        Add(controller);

        // If spawned above ground, animate fall
        if (position.y > GroundY)
        {
            AnimateFall(go.transform, position.y);
        }

        // Notify listeners about the new worker
        OnMonsterSpawned?.Invoke(controller.EntityId, type);

        return controller;
    }

    private const float GroundY = -3f;

    private void AnimateFall(Transform target, float startY)
    {
        float fallDistance = startY - GroundY;
        float fallDuration = fallDistance * 0.15f; // ~0.75s for 5 units
        target.DOMoveY(GroundY, fallDuration).SetEase(Ease.InQuad);
    }

    /// <summary>
    /// Gets the count of workers currently assigned to workspaces.
    /// </summary>
    public int GetWorkingCount()
    {
        int count = 0;
        foreach (var controller in _skeletonControllers)
        {
            if (controller.State == SkeletonState.Working)
                count++;
        }
        return count;
    }

    /// <summary>
    /// Gets workers by their assigned workspace state.
    /// </summary>
    public List<WorkhorseController> GetWorkingWorkers()
    {
        var result = new List<WorkhorseController>();
        foreach (var controller in _skeletonControllers)
        {
            if (controller.State == SkeletonState.Working)
                result.Add(controller);
        }
        return result;
    }

    /// <summary>
    /// Destroys a worker and removes it from all managers.
    /// </summary>
    public void DestroySkeleton(WorkhorseController controller)
    {
        if (controller == null)
            return;

        // Unassign from workspace if assigned
        if (controller.AssignedWorkspaceId.HasValue)
        {
            var workspace = WorkspaceControllers.Instance.GetByEntityId(controller.AssignedWorkspaceId.Value);
            workspace?.UnassignSkeleton();
        }

        // Unregister from animator system
        WorkhorseAnimator.Unregister(controller.EntityId);

        // Remove from controller list
        Remove(controller);

        // Destroy the GameObject
        if (controller.Transform != null)
        {
            UnityEngine.Object.Destroy(controller.Transform.gameObject);
        }
    }
}
