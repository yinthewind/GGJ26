using System;
using UnityEngine;

public class LevelManager
{
    public static LevelManager Instance { get; } = new();

    private string _currentLevelId;
    private LevelConfig _currentConfig;
    private int _turnsRemaining;
    private bool _isLevelComplete;
    private bool _isLevelFailed;

    public string CurrentLevelId => _currentLevelId;
    public LevelConfig CurrentConfig => _currentConfig;
    public int TurnsRemaining => _turnsRemaining;
    public bool IsLevelComplete => _isLevelComplete;
    public bool IsLevelFailed => _isLevelFailed;

    // Events
    public event Action<string> OnLevelLoaded;
    public event Action OnLevelWon;
    public event Action OnLevelFailed;
    public event Action<int> OnTurnsRemainingChanged;
    public event Action<int> OnProjectRewardEarned;

    public void LoadLevel(string levelId)
    {
        var config = LevelDefinitions.GetLevel(levelId);
        if (config == null)
        {
            Debug.LogError($"LevelManager: Level '{levelId}' not found");
            return;
        }

        // Save game state BEFORE clearing
        PlayerProgress.Instance.SaveGameState(
            WorkspaceControllers.Instance.Workspaces,
            CharacterControllers.Instance.Skeletons);

        // Clear existing game state
        ClearGameState();

        // Set up new level
        _currentLevelId = levelId;
        _currentConfig = config;
        _turnsRemaining = config.TurnLimit;
        _isLevelComplete = false;
        _isLevelFailed = false;

        // Reset player progress
        PlayerProgress.Instance.Reset();

        // Reset turn manager
        TurnManager.Instance.Reset();

        // Load goal from level config
        GoalManager.Instance.LoadGoalFromLevel(_currentConfig);

        // Spawn initial setup
        SpawnInitialSetup();

        OnLevelLoaded?.Invoke(levelId);
        OnTurnsRemainingChanged?.Invoke(_turnsRemaining);
    }

    public void RestartLevel()
    {
        if (_currentLevelId != null)
        {
            LoadLevel(_currentLevelId);
        }
    }

    public void OnTurnEnded()
    {
        if (_isLevelComplete || _isLevelFailed)
            return;

        // Decrement turns
        _turnsRemaining--;
        OnTurnsRemainingChanged?.Invoke(_turnsRemaining);

        // Check win condition via GoalManager
        if (GoalManager.Instance.CurrentGoal?.IsCompleted == true)
        {
            _isLevelComplete = true;

            // Award project completion reward
            int reward = _currentConfig.ProjectReward;
            if (reward > 0)
            {
                PlayerProgress.Instance.AddDollar(reward);
                OnProjectRewardEarned?.Invoke(reward);
            }

            OnLevelWon?.Invoke();
            return;
        }

        // Check lose condition
        if (_turnsRemaining <= 0)
        {
            _isLevelFailed = true;
            OnLevelFailed?.Invoke();
        }
    }

    public bool IsWorkhorseTypeAvailable(WorkhorseType type)
    {
        if (_currentConfig == null)
            return true;

        return _currentConfig.AvailableWorkhorseTypes.Contains(type);
    }

    private void ClearGameState()
    {
        // Destroy all workhorse GameObjects
        var skeletons = CharacterControllers.Instance.Skeletons;
        for (int i = skeletons.Count - 1; i >= 0; i--)
        {
            var skeleton = skeletons[i];
            if (skeleton.Transform != null)
            {
                UnityEngine.Object.Destroy(skeleton.Transform.gameObject);
            }
        }
        CharacterControllers.Instance.Clear();

        // Destroy all workspace GameObjects
        var workspaces = WorkspaceControllers.Instance.Workspaces;
        for (int i = workspaces.Count - 1; i >= 0; i--)
        {
            var workspace = workspaces[i];
            var animator = workspace.Animator;
            if (animator != null && animator.gameObject != null)
            {
                UnityEngine.Object.Destroy(animator.gameObject);
            }
        }
        WorkspaceControllers.Instance.Clear();

        // No entity ID counter reset needed - using GUIDs now
    }

    private void SpawnInitialSetup()
    {
        var savedWorkspaces = PlayerProgress.Instance.WorkspaceData;
        var savedWorkhorses = PlayerProgress.Instance.WorkhorseData;

        if (savedWorkspaces.Count > 0)
        {
            // Restore workspaces at saved positions with their original GUIDs
            for (int i = 0; i < savedWorkspaces.Count; i++)
            {
                var data = savedWorkspaces[i];
                var worldPos = GridSystem.GridToWorld(data.GridPosition);
                var workspace = WorkspaceControllers.Instance.SpawnWorkspaceWithId(
                    data.EntityId, worldPos, new Vector2Int(1, 1), WorkspaceType.Basic,
                    $"Workspace{i + 1}");
                Debug.Log($"[Restore] Workspace EntityId {workspace.EntityId} at grid {data.GridPosition}, world {worldPos}, center {workspace.WorldCenter}");
            }

            // Restore workhorses with their assignments - look up by saved workspace GUID
            foreach (var data in savedWorkhorses)
            {
                Vector3 spawnPos;
                WorkspaceController targetWorkspace = null;

                if (data.AssignedWorkspaceId.HasValue)
                {
                    targetWorkspace = WorkspaceControllers.Instance.GetByEntityId(data.AssignedWorkspaceId.Value);
                }

                if (targetWorkspace != null)
                {
                    // Spawn at ground level to avoid triggering fall animation
                    // AssignToWorkspace will position at workspace center
                    spawnPos = new Vector3(0f, -3f, 0f);
                }
                else
                {
                    // Unassigned: use saved position
                    spawnPos = data.Position;
                }

                var workhorse = CharacterControllers.Instance.SpawnSkeletonWithId(data.EntityId, data.Type, spawnPos);

                // Restore reveal state
                if (data.IsRevealed)
                {
                    workhorse.Reveal();
                }

                // Re-establish assignment if applicable
                if (targetWorkspace != null)
                {
                    Debug.Log($"[Restore] Workhorse {workhorse.EntityId} ({data.Type}) -> workspace {targetWorkspace.EntityId}, WorldCenter {targetWorkspace.WorldCenter}");
                    targetWorkspace.AssignSkeleton(workhorse.EntityId);
                    workhorse.AssignToWorkspace(targetWorkspace.EntityId, targetWorkspace.WorldCenter);
                }
            }
        }
        else
        {
            // First level: spawn default 3 workspaces in L shape
            WorkspaceControllers.Instance.SpawnWorkspace(
                new Vector3(0f, -1f, 0f), new Vector2Int(1, 1), WorkspaceType.Basic, "Workspace1");
            WorkspaceControllers.Instance.SpawnWorkspace(
                new Vector3(1f, -1f, 0f), new Vector2Int(1, 1), WorkspaceType.Basic, "Workspace2");
            WorkspaceControllers.Instance.SpawnWorkspace(
                new Vector3(1f, 0f, 0f), new Vector2Int(1, 1), WorkspaceType.Basic, "Workspace3");

            // Spawn initial workers based on available types
            var availableTypes = _currentConfig.AvailableWorkhorseTypes;
            if (availableTypes.Count > 0)
            {
                CharacterControllers.Instance.SpawnSkeleton(availableTypes[0], new Vector3(-3f, 1f, 0f));
            }
            if (availableTypes.Count > 1)
            {
                CharacterControllers.Instance.SpawnSkeleton(availableTypes[1], new Vector3(3f, 1f, 0f));
            }
        }
    }
}
