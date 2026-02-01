using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Saboteur (破坏家): -15% total team income.
/// Actively undermines the team's overall productivity.
/// </summary>
public class SaboteurSynergy : AbilitySynergy
{
    public override string Name => "破坏家";
    public override WorkhorseType WorkhorseType => WorkhorseType.Saboteur;
    public override float BonusPercent => -15f;  // For display purposes
    public override string Description => "总团队收入-15%（最多减少70%）";

    private const float TotalPenalty = 0.85f;  // -15%
    private const float MaxPenalty = 0.7f;     // Max 70% reduction (floor at 30% productivity)

    public override float GetTotalMultiplier(List<WorkhorseAssignment> assignments)
    {
        int saboteurCount = assignments.Count(a => a.Type == WorkhorseType.Saboteur);
        if (saboteurCount == 0)
            return 1.0f;

        // Stack multiplicatively for multiple saboteurs
        float multiplier = 1.0f;
        for (int i = 0; i < saboteurCount; i++)
        {
            multiplier *= TotalPenalty;
        }
        return Mathf.Max(1f - MaxPenalty, multiplier);  // Floor at 0.3
    }
}
