using System.Collections.Generic;
using UnityEngine;

public static class ProductivityCalculator
{
    /// <summary>
    /// Calculates total productivity including synergy bonuses.
    /// Formula: Total Base × (1 + Sum of Synergy Bonuses)
    /// </summary>
    public static float CalculateProductivity(
        List<Workspace> workspaces,
        List<WorkhorseAssignment> assignments,
        ISynergyReference synergyRef)
    {
        if (assignments.Count == 0)
            return 0f;

        // Calculate base productivity
        float totalBase = 0f;
        var workspaceDict = BuildWorkspaceDictionary(workspaces);

        foreach (var assignment in assignments)
        {
            if (workspaceDict.TryGetValue(assignment.WorkspaceId, out var workspace))
            {
                totalBase += GameSettings.CalculateProductivity(workspace.Type, assignment.Type);
            }
        }

        // Calculate synergy bonus
        float synergyBonus = 0f;
        var activeSynergies = SynergySystem.Instance.GetActiveSynergies(workspaces, assignments);

        foreach (var synergy in activeSynergies)
        {
            if (synergy.IsActive)
            {
                synergyBonus += synergy.Synergy.BonusPercent / 100f;
            }
        }

        // Apply synergy multiplier
        float finalProductivity = totalBase * (1f + synergyBonus);

        return finalProductivity;
    }

    /// <summary>
    /// Calculates base productivity without synergy bonuses.
    /// </summary>
    public static float CalculateBaseProductivity(
        List<Workspace> workspaces,
        List<WorkhorseAssignment> assignments)
    {
        if (assignments.Count == 0)
            return 0f;

        float totalBase = 0f;
        var workspaceDict = BuildWorkspaceDictionary(workspaces);

        foreach (var assignment in assignments)
        {
            if (workspaceDict.TryGetValue(assignment.WorkspaceId, out var workspace))
            {
                totalBase += GameSettings.CalculateProductivity(workspace.Type, assignment.Type);
            }
        }

        return totalBase;
    }

    /// <summary>
    /// Gets productivity breakdown per worker.
    /// </summary>
    public static Dictionary<int, float> GetProductivityBreakdown(
        List<Workspace> workspaces,
        List<WorkhorseAssignment> assignments)
    {
        var breakdown = new Dictionary<int, float>();
        var workspaceDict = BuildWorkspaceDictionary(workspaces);

        foreach (var assignment in assignments)
        {
            if (workspaceDict.TryGetValue(assignment.WorkspaceId, out var workspace))
            {
                breakdown[assignment.WorkerId] = GameSettings.CalculateProductivity(workspace.Type, assignment.Type);
            }
        }

        return breakdown;
    }

    private static Dictionary<int, Workspace> BuildWorkspaceDictionary(List<Workspace> workspaces)
    {
        var dict = new Dictionary<int, Workspace>();
        foreach (var workspace in workspaces)
        {
            dict[workspace.Id] = workspace;
        }
        return dict;
    }
}
