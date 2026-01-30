using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Base class for workhorse ability synergies that modify productivity calculations.
/// Unlike regular synergies that apply a global bonus, ability synergies can modify
/// individual worker productivity, adjacent worker productivity, or total productivity.
/// </summary>
public abstract class AbilitySynergy : Synergy
{
    public override SynergyType Type => SynergyType.WorkhorseAbility;

    /// <summary>
    /// The workhorse type this ability is associated with.
    /// </summary>
    public abstract WorkhorseType WorkhorseType { get; }

    /// <summary>
    /// Gets the productivity modifier for the worker with this ability.
    /// Returns the absolute productivity value (not a multiplier).
    /// </summary>
    public virtual float GetSelfProductivity(
        WorkhorseAssignment self,
        float baseProductivity,
        List<Workspace> workspaces,
        List<WorkhorseAssignment> allAssignments)
    {
        return baseProductivity;
    }

    /// <summary>
    /// Gets the productivity multiplier to apply to an adjacent worker.
    /// Returns 1.0 for no change, 1.5 for +50%, 0.7 for -30%, etc.
    /// </summary>
    public virtual float GetAdjacentMultiplier(
        WorkhorseAssignment self,
        WorkhorseAssignment adjacent,
        List<Workspace> workspaces)
    {
        return 1.0f;
    }

    /// <summary>
    /// Gets the total productivity multiplier to apply to the entire team.
    /// Returns 1.0 for no change, 0.85 for -15%, etc.
    /// </summary>
    public virtual float GetTotalMultiplier(
        List<WorkhorseAssignment> assignments)
    {
        return 1.0f;
    }

    /// <summary>
    /// Check if this synergy is active (when the workhorse type is assigned).
    /// </summary>
    public override bool CheckActive(List<Workspace> workspaces, List<WorkhorseAssignment> assignments)
    {
        return assignments.Any(a => a.Type == WorkhorseType);
    }

    /// <summary>
    /// Helper to check if two workers are adjacent based on their workspace positions.
    /// </summary>
    protected bool AreWorkersAdjacent(
        WorkhorseAssignment a1,
        WorkhorseAssignment a2,
        List<Workspace> workspaces)
    {
        var workspaceDict = workspaces.ToDictionary(w => w.Id);

        if (!workspaceDict.TryGetValue(a1.WorkspaceId, out var w1)) return false;
        if (!workspaceDict.TryGetValue(a2.WorkspaceId, out var w2)) return false;

        return SynergyHelpers.AreWorkspacesAdjacent(w1, w2);
    }

    /// <summary>
    /// Helper to check if a worker has any adjacent workers.
    /// </summary>
    protected bool HasAdjacentWorkers(
        WorkhorseAssignment self,
        List<Workspace> workspaces,
        List<WorkhorseAssignment> allAssignments)
    {
        foreach (var other in allAssignments)
        {
            if (other.WorkerId == self.WorkerId) continue;
            if (AreWorkersAdjacent(self, other, workspaces))
                return true;
        }
        return false;
    }
}
