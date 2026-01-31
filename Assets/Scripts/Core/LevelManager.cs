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

        // Reset entity ID counters
        WorkhorseController.ResetEntityIdCounter();
        WorkspaceController.ResetEntityIdCounter();
    }

    private void SpawnInitialSetup()
    {
        // Spawn 3 basic workspaces in L shape
        WorkspaceControllers.Instance.SpawnWorkspace(
            new Vector3(0f, 1f, 0f), new Vector2Int(1, 1), WorkspaceType.Basic, "Workspace1");
        WorkspaceControllers.Instance.SpawnWorkspace(
            new Vector3(1f, 1f, 0f), new Vector2Int(1, 1), WorkspaceType.Basic, "Workspace2");
        WorkspaceControllers.Instance.SpawnWorkspace(
            new Vector3(1f, 2f, 0f), new Vector2Int(1, 1), WorkspaceType.Basic, "Workspace3");

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
