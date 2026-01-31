using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ProductivityCalculator
{
    /// <summary>
    /// Calculates total productivity including synergy bonuses and ability effects.
    /// Formula:
    /// 1. Calculate base productivity per worker
    /// 2. Apply self-modifying ability effects (ToxicWolf, RisingStar, Pessimist)
    /// 3. Apply adjacency multipliers (Encourager +50%, FreeSpirit -30%)
    /// 4. Sum all worker productivities
    /// 5. Apply regular synergy bonuses
    /// 6. Apply global ability effects (Saboteur -15%)
    /// </summary>
    public static float CalculateProductivity(
        List<Workspace> workspaces,
        List<WorkhorseAssignment> assignments,
        ISynergyReference synergyRef)
    {
        if (assignments.Count == 0)
            return 0f;

        var workspaceDict = BuildWorkspaceDictionary(workspaces);
        var abilitySynergies = synergyRef.AllSynergies.OfType<AbilitySynergy>().ToList();

        // Step 1-3: Calculate productivity per worker with ability effects
        var workerProductivities = new Dictionary<Guid, float>();

        foreach (var assignment in assignments)
        {
            if (!workspaceDict.TryGetValue(assignment.WorkspaceId, out var workspace))
                continue;

            // Step 1: Base productivity
            float productivity = GameSettings.CalculateProductivity(workspace.Type, assignment.Type);

            // Step 2: Apply self-modifying ability effects
            foreach (var ability in abilitySynergies)
            {
                productivity = ability.GetSelfProductivity(assignment, productivity, workspaces, assignments);
            }

            // Apply workspace multiplier to ability-modified productivity
            // (Only if the ability didn't override the base calculation)
            // Note: Abilities return absolute values, so we need to apply workspace multiplier
            float workspaceMultiplier = GameSettings.WorkspaceProductivityMultipliers[workspace.Type];

            // Check if this is an ability-modified worker
            bool isAbilityWorker = IsAbilityWorker(assignment.Type);
            if (isAbilityWorker)
            {
                // For ability workers, apply workspace multiplier to the ability result
                productivity *= workspaceMultiplier;
            }

            workerProductivities[assignment.WorkerId] = productivity;
        }

        // Step 3: Apply adjacency multipliers
        foreach (var assignment in assignments)
        {
            if (!workerProductivities.ContainsKey(assignment.WorkerId))
                continue;

            float adjacencyMultiplier = 1.0f;

            // Check all ability synergies for adjacency effects on this worker
            foreach (var otherAssignment in assignments)
            {
                if (otherAssignment.WorkerId == assignment.WorkerId)
                    continue;

                foreach (var ability in abilitySynergies)
                {
                    float multiplier = ability.GetAdjacentMultiplier(otherAssignment, assignment, workspaces);
                    adjacencyMultiplier *= multiplier;
                }
            }

            workerProductivities[assignment.WorkerId] *= adjacencyMultiplier;
        }

        // Step 4: Sum all worker productivities
        float totalBase = workerProductivities.Values.Sum();

        // Step 5: Calculate regular synergy bonus (non-ability synergies)
        float synergyBonus = 0f;
        var activeSynergies = SynergySystem.Instance.GetActiveSynergies(workspaces, assignments);

        foreach (var synergy in activeSynergies)
        {
            if (synergy.IsActive && synergy.Synergy.Type != SynergyType.WorkhorseAbility)
            {
                synergyBonus += synergy.Synergy.BonusPercent / 100f;
            }
        }

        float afterSynergies = totalBase * (1f + synergyBonus);

        // Step 6: Apply global ability effects (Saboteur)
        float totalMultiplier = 1.0f;
        foreach (var ability in abilitySynergies)
        {
            totalMultiplier *= ability.GetTotalMultiplier(assignments);
        }

        float finalProductivity = afterSynergies * totalMultiplier;

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
    /// Gets productivity breakdown per worker including ability effects.
    /// </summary>
    public static Dictionary<Guid, float> GetProductivityBreakdown(
        List<Workspace> workspaces,
        List<WorkhorseAssignment> assignments)
    {
        var breakdown = new Dictionary<Guid, float>();
        var workspaceDict = BuildWorkspaceDictionary(workspaces);
        var abilitySynergies = SynergySystem.Instance.AllSynergies.OfType<AbilitySynergy>().ToList();

        // Calculate base productivity with self-modifying effects
        foreach (var assignment in assignments)
        {
            if (!workspaceDict.TryGetValue(assignment.WorkspaceId, out var workspace))
                continue;

            float productivity = GameSettings.CalculateProductivity(workspace.Type, assignment.Type);

            // Apply self-modifying ability effects
            foreach (var ability in abilitySynergies)
            {
                productivity = ability.GetSelfProductivity(assignment, productivity, workspaces, assignments);
            }

            // Apply workspace multiplier for ability workers
            float workspaceMultiplier = GameSettings.WorkspaceProductivityMultipliers[workspace.Type];
            if (IsAbilityWorker(assignment.Type))
            {
                productivity *= workspaceMultiplier;
            }

            breakdown[assignment.WorkerId] = productivity;
        }

        // Apply adjacency multipliers
        foreach (var assignment in assignments)
        {
            if (!breakdown.ContainsKey(assignment.WorkerId))
                continue;

            float adjacencyMultiplier = 1.0f;

            foreach (var otherAssignment in assignments)
            {
                if (otherAssignment.WorkerId == assignment.WorkerId)
                    continue;

                foreach (var ability in abilitySynergies)
                {
                    float multiplier = ability.GetAdjacentMultiplier(otherAssignment, assignment, workspaces);
                    adjacencyMultiplier *= multiplier;
                }
            }

            breakdown[assignment.WorkerId] *= adjacencyMultiplier;
        }

        return breakdown;
    }

    private static Dictionary<Guid, Workspace> BuildWorkspaceDictionary(List<Workspace> workspaces)
    {
        var dict = new Dictionary<Guid, Workspace>();
        foreach (var workspace in workspaces)
        {
            dict[workspace.Id] = workspace;
        }
        return dict;
    }

    /// <summary>
    /// Checks if the workhorse type has a special ability that overrides base productivity.
    /// </summary>
    private static bool IsAbilityWorker(WorkhorseType type)
    {
        return type == WorkhorseType.ToxicWolf ||
               type == WorkhorseType.RisingStar ||
               type == WorkhorseType.Pessimist;
    }
}
