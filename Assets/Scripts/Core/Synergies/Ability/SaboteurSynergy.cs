using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Saboteur (破坏家): -15% total team income.
/// Actively undermines the team's overall productivity.
/// </summary>
public class SaboteurSynergy : AbilitySynergy
{
    public override string Name => "Saboteur";
    public override WorkhorseType WorkhorseType => WorkhorseType.Saboteur;
    public override float BonusPercent => -15f;  // For display purposes
    public override string Description => "-15% total team income";

    private const float TotalPenalty = 0.85f;  // -15%

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
        return multiplier;
    }
}
