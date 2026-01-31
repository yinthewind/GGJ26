using System;
using UnityEngine;

public enum GoalType
{
    TotalProductivity,
    WorkerCount,
    SynergyCount
}

public class GoalDefinition
{
    public string Id;
    public string Name;
    public string Description;
    public GoalType Type;
    public float TargetValue;
    public bool IsCompleted;
}

public class GoalManager
{
    public static GoalManager Instance { get; } = new();

    private GoalDefinition _currentGoal;

    // Events
    public event Action<GoalDefinition> OnGoalCompleted;
    public event Action<GoalDefinition> OnGoalChanged;

    public GoalDefinition CurrentGoal => _currentGoal;

    public void LoadGoalFromLevel(LevelConfig config)
    {
        if (config == null)
        {
            _currentGoal = null;
            return;
        }

        _currentGoal = new GoalDefinition
        {
            Id = config.LevelId + "_goal",
            Name = config.GoalName,
            Description = config.GoalDescription,
            Type = config.GoalType,
            TargetValue = config.GoalTargetValue,
            IsCompleted = false
        };

        OnGoalChanged?.Invoke(_currentGoal);
    }

    public void CheckGoals()
    {
        if (_currentGoal == null || _currentGoal.IsCompleted)
            return;

        bool completed = false;

        switch (_currentGoal.Type)
        {
            case GoalType.TotalProductivity:
                completed = PlayerProgress.Instance.TotalProductivity >= _currentGoal.TargetValue;
                break;

            case GoalType.WorkerCount:
                completed = CharacterControllers.Instance.Skeletons.Count >= _currentGoal.TargetValue;
                break;

            case GoalType.SynergyCount:
                var synergies = TurnManager.Instance.PreviewSynergies();
                int activeSynergyCount = 0;
                foreach (var synergy in synergies)
                {
                    if (synergy.IsActive)
                        activeSynergyCount++;
                }
                completed = activeSynergyCount >= _currentGoal.TargetValue;
                PlayerProgress.Instance.UpdateMaxSynergies(activeSynergyCount);
                break;
        }

        if (completed)
        {
            CompleteCurrentGoal();
        }
    }

    private void CompleteCurrentGoal()
    {
        if (_currentGoal == null)
            return;

        _currentGoal.IsCompleted = true;
        OnGoalCompleted?.Invoke(_currentGoal);

        Debug.Log($"Goal completed: {_currentGoal.Name}!");
    }

    public float GetGoalProgress()
    {
        if (_currentGoal == null)
            return 1f;

        float currentValue = 0f;

        switch (_currentGoal.Type)
        {
            case GoalType.TotalProductivity:
                currentValue = PlayerProgress.Instance.TotalProductivity;
                break;

            case GoalType.WorkerCount:
                currentValue = CharacterControllers.Instance.Skeletons.Count;
                break;

            case GoalType.SynergyCount:
                currentValue = PlayerProgress.Instance.MaxSynergiesInOneTurn;
                break;
        }

        return Mathf.Clamp01(currentValue / _currentGoal.TargetValue);
    }

    public void Reset()
    {
        // Reload goal from current level config
        var currentConfig = LevelManager.Instance.CurrentConfig;
        if (currentConfig != null)
        {
            LoadGoalFromLevel(currentConfig);
        }
        else
        {
            _currentGoal = null;
        }
    }
}
