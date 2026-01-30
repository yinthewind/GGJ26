using System;
using System.Collections.Generic;
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
    public string Reward;
    public bool IsCompleted;
}

public class GoalManager
{
    public static GoalManager Instance { get; } = new();

    private readonly List<GoalDefinition> _goals = new();
    private int _currentGoalIndex = 0;

    // Events
    public event Action<GoalDefinition> OnGoalCompleted;
    public event Action<GoalDefinition> OnGoalChanged;

    public GoalDefinition CurrentGoal => _currentGoalIndex < _goals.Count ? _goals[_currentGoalIndex] : null;
    public IReadOnlyList<GoalDefinition> AllGoals => _goals;

    public GoalManager()
    {
        InitializeGoals();
    }

    private void InitializeGoals()
    {
        _goals.Clear();

        _goals.Add(new GoalDefinition
        {
            Id = "first_steps",
            Name = "First Steps",
            Description = "Generate 10 total productivity",
            Type = GoalType.TotalProductivity,
            TargetValue = 10f,
            Reward = "Foundation bonus",
            IsCompleted = false
        });

        _goals.Add(new GoalDefinition
        {
            Id = "growing_team",
            Name = "Growing Team",
            Description = "Have 4 workers",
            Type = GoalType.WorkerCount,
            TargetValue = 4f,
            Reward = "Unlock Mage",
            IsCompleted = false
        });

        _goals.Add(new GoalDefinition
        {
            Id = "productivity_boost",
            Name = "Productivity Boost",
            Description = "Generate 50 total productivity",
            Type = GoalType.TotalProductivity,
            TargetValue = 50f,
            Reward = "Unlock Advanced workspace",
            IsCompleted = false
        });

        _goals.Add(new GoalDefinition
        {
            Id = "synergy_master",
            Name = "Synergy Master",
            Description = "Activate 3 synergies in one turn",
            Type = GoalType.SynergyCount,
            TargetValue = 3f,
            Reward = "Synergy bonus",
            IsCompleted = false
        });

        _goals.Add(new GoalDefinition
        {
            Id = "powerhouse",
            Name = "Powerhouse",
            Description = "Generate 100 total productivity",
            Type = GoalType.TotalProductivity,
            TargetValue = 100f,
            Reward = "Elite workspace",
            IsCompleted = false
        });
    }

    public void CheckGoals()
    {
        var currentGoal = CurrentGoal;
        if (currentGoal == null || currentGoal.IsCompleted)
            return;

        bool completed = false;

        switch (currentGoal.Type)
        {
            case GoalType.TotalProductivity:
                completed = PlayerProgress.Instance.TotalProductivity >= currentGoal.TargetValue;
                break;

            case GoalType.WorkerCount:
                completed = CharacterControllers.Instance.Skeletons.Count >= currentGoal.TargetValue;
                break;

            case GoalType.SynergyCount:
                var synergies = TurnManager.Instance.PreviewSynergies();
                int activeSynergyCount = 0;
                foreach (var synergy in synergies)
                {
                    if (synergy.IsActive)
                        activeSynergyCount++;
                }
                completed = activeSynergyCount >= currentGoal.TargetValue;
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
        var goal = CurrentGoal;
        if (goal == null)
            return;

        goal.IsCompleted = true;
        OnGoalCompleted?.Invoke(goal);

        // Apply reward
        ApplyReward(goal);

        // Move to next goal
        _currentGoalIndex++;

        var nextGoal = CurrentGoal;
        if (nextGoal != null)
        {
            OnGoalChanged?.Invoke(nextGoal);
        }
    }

    private void ApplyReward(GoalDefinition goal)
    {
        // Rewards can be implemented here
        // For now, just log the reward
        Debug.Log($"Goal completed: {goal.Name}! Reward: {goal.Reward}");
    }

    public float GetGoalProgress()
    {
        var goal = CurrentGoal;
        if (goal == null)
            return 1f;

        float currentValue = 0f;

        switch (goal.Type)
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

        return Mathf.Clamp01(currentValue / goal.TargetValue);
    }

    public void Reset()
    {
        foreach (var goal in _goals)
        {
            goal.IsCompleted = false;
        }
        _currentGoalIndex = 0;

        var firstGoal = CurrentGoal;
        if (firstGoal != null)
        {
            OnGoalChanged?.Invoke(firstGoal);
        }
    }
}
