using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Pessimist (消极派): 100 - 30n income, where n = rounds worked (min 0).
/// Becomes less productive over time due to burnout.
/// </summary>
public class PessimistSynergy : AbilitySynergy
{
    public override string Name => "Pessimist";
    public override WorkhorseType WorkhorseType => WorkhorseType.Pessimist;
    public override float BonusPercent => 0f;
    public override string Description => "-30 income each round (100 - 30n, min 0)";

    private const float BaseProductivity = 1.0f;      // 100 base
    private const float PerRoundPenalty = 0.3f;       // -30 per round

    public override float GetSelfProductivity(
        WorkhorseAssignment self,
        float baseProductivity,
        List<Workspace> workspaces,
        List<WorkhorseAssignment> allAssignments)
    {
        if (self.Type != WorkhorseType.Pessimist)
            return baseProductivity;

        float result = BaseProductivity - (self.RoundsWorked * PerRoundPenalty);
        return Mathf.Max(0f, result);
    }
}
