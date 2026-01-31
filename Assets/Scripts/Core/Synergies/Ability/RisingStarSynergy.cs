using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// RisingStar (潜力股): 50 + 50n income, where n = rounds worked.
/// Grows stronger each turn they stay assigned.
/// </summary>
public class RisingStarSynergy : AbilitySynergy
{
    public override string Name => "Rising Star";
    public override WorkhorseType WorkhorseType => WorkhorseType.RisingStar;
    public override float BonusPercent => 0f;
    public override string Description => "+50 income each round (50 + 50n)";

    private const float BaseProductivity = 0.5f;      // 50 base
    private const float PerRoundBonus = 0.5f;         // +50 per round
    private const float MaxProductivity = 2.5f;       // Max: 250

    public override float GetSelfProductivity(
        WorkhorseAssignment self,
        float baseProductivity,
        List<Workspace> workspaces,
        List<WorkhorseAssignment> allAssignments)
    {
        if (self.Type != WorkhorseType.RisingStar)
            return baseProductivity;

        float result = BaseProductivity + (self.RoundsWorked * PerRoundBonus);
        return Mathf.Min(result, MaxProductivity);
    }
}
