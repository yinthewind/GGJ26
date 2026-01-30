using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public struct ManaGenerationEntry
{
    public int SkeletonId;
    public float Amount;
}

public class CharacterControllers
{
    public static CharacterControllers Instance { get; } = new();

    private readonly List<WorkhorseController> _skeletonControllers = new();

    private float _manaTimer = 0f;

    // Event fired when mana is generated
    // Parameters: List of (skeletonId, manaAmount) entries
    public event Action<List<ManaGenerationEntry>> OnManaGenerated;

    // Event fired when a monster is spawned
    // Parameters: entityId, skeletonType
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

        // Global mana generation
        _manaTimer += deltaTime;
        if (_manaTimer >= GameSettings.ManaGenerationInterval)
        {
            _manaTimer = 0f;
            GenerateMana();
        }
    }

    private void GenerateMana()
    {
        var entries = new List<ManaGenerationEntry>();

        foreach (var skeleton in _skeletonControllers)
        {
            if (skeleton.State != SkeletonState.Working || !skeleton.AssignedWorkspaceId.HasValue)
                continue;

            var workspace = WorkspaceControllers.Instance.GetByEntityId(skeleton.AssignedWorkspaceId.Value);
            if (workspace == null)
                continue;

            var mana = GameSettings.CalculateMana(workspace.Type, skeleton.Type);
            entries.Add(new ManaGenerationEntry { SkeletonId = skeleton.EntityId, Amount = mana });
        }

        if (entries.Count > 0)
        {
            OnManaGenerated?.Invoke(entries);
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
    /// Spawns a new skeleton at the specified position.
    /// Creates the visual, controller, registers with all managers, and updates PlayProgress.
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
        if (position.y > 0f)
        {
            AnimateFall(go.transform, position.y);
        }

        // Notify listeners about the new monster
        OnMonsterSpawned?.Invoke(controller.EntityId, type);

        return controller;
    }

    private void AnimateFall(Transform target, float startY)
    {
        float fallDuration = startY * 0.15f; // ~0.75s for 5 units
        target.DOMoveY(0f, fallDuration).SetEase(Ease.InQuad);
    }
}
