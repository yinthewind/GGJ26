using System;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager
{
    public static TurnManager Instance { get; } = new();

    private int _currentTurn = 1;
    private bool _isResolvingTurn = false;

    public int CurrentTurn => _currentTurn;
    public bool IsResolvingTurn => _isResolvingTurn;

    // Events
    public event Action OnTurnStarted;
    public event Action OnTurnEnded;
    public event Action<float> OnProductivityGained;
    public event Action<List<SynergyResult>> OnSynergiesActivated;

    /// <summary>
    /// Ends the current turn and resolves productivity.
    /// </summary>
    public void EndTurn()
    {
        if (_isResolvingTurn)
            return;

        _isResolvingTurn = true;

        // Calculate productivity
        var workspaces = BuildWorkspaceList();
        var assignments = BuildAssignmentList();

        float productivity = ProductivityCalculator.CalculateProductivity(
            workspaces,
            assignments,
            SynergySystem.Instance);

        // Get active synergies for UI feedback
        var synergies = SynergySystem.Instance.GetActiveSynergies(workspaces, assignments);
        if (synergies.Count > 0)
        {
            OnSynergiesActivated?.Invoke(synergies);
        }

        // Add to player progress
        if (productivity > 0)
        {
            PlayerProgress.Instance.AddProductivity(productivity);
            OnProductivityGained?.Invoke(productivity);

            // Award gold based on productivity
            int goldEarned = Mathf.RoundToInt(productivity);
            if (goldEarned > 0)
            {
                PlayerProgress.Instance.AddGold(goldEarned);
            }
        }

        // Increment rounds worked for all working skeletons
        foreach (var skeleton in CharacterControllers.Instance.Skeletons)
        {
            if (skeleton.State == SkeletonState.Working && skeleton.AssignedWorkspaceId.HasValue)
            {
                skeleton.IncrementRoundsWorked();
            }
        }

        OnTurnEnded?.Invoke();

        // Advance turn
        _currentTurn++;
        _isResolvingTurn = false;

        OnTurnStarted?.Invoke();

        // Check goals after turn resolution
        GoalManager.Instance?.CheckGoals();

        // Notify level manager for win/lose checks
        LevelManager.Instance.OnTurnEnded();
    }

    /// <summary>
    /// Calculates preview productivity without ending the turn.
    /// </summary>
    public float PreviewProductivity()
    {
        var workspaces = BuildWorkspaceList();
        var assignments = BuildAssignmentList();

        return ProductivityCalculator.CalculateProductivity(
            workspaces,
            assignments,
            SynergySystem.Instance);
    }

    /// <summary>
    /// Gets active synergies for preview display.
    /// </summary>
    public List<SynergyResult> PreviewSynergies()
    {
        var workspaces = BuildWorkspaceList();
        var assignments = BuildAssignmentList();

        return SynergySystem.Instance.GetActiveSynergies(workspaces, assignments);
    }

    private List<Workspace> BuildWorkspaceList()
    {
        var workspaces = new List<Workspace>();

        foreach (var controller in WorkspaceControllers.Instance.Workspaces)
        {
            workspaces.Add(new Workspace
            {
                Id = controller.EntityId,
                Type = controller.Type,
                Position = controller.GridPosition,
                Size = controller.GridSize
            });
        }

        return workspaces;
    }

    private List<WorkhorseAssignment> BuildAssignmentList()
    {
        var assignments = new List<WorkhorseAssignment>();

        foreach (var skeleton in CharacterControllers.Instance.Skeletons)
        {
            if (skeleton.State == SkeletonState.Working && skeleton.AssignedWorkspaceId.HasValue)
            {
                assignments.Add(new WorkhorseAssignment
                {
                    WorkerId = skeleton.EntityId,
                    WorkspaceId = skeleton.AssignedWorkspaceId.Value,
                    Type = skeleton.Type,
                    RoundsWorked = skeleton.RoundsWorked
                });
            }
        }

        return assignments;
    }

    public void Reset()
    {
        _currentTurn = 1;
        _isResolvingTurn = false;
    }
}
