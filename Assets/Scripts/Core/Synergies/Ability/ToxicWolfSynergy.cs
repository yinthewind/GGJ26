using System.Collections.Generic;

/// <summary>
/// ToxicWolf (毒狼): 200 income if no adjacent workers, 50 otherwise.
/// Works best alone, penalized when crowded.
/// </summary>
public class ToxicWolfSynergy : AbilitySynergy
{
    public override string Name => "Toxic Wolf";
    public override WorkhorseType WorkhorseType => WorkhorseType.ToxicWolf;
    public override float BonusPercent => 0f;
    public override string Description => "200 income alone, 50 if adjacent to others";

    private const float AloneProductivity = 2.0f;    // 200 base
    private const float CrowdedProductivity = 0.5f;  // 50 base

    public override float GetSelfProductivity(
        WorkhorseAssignment self,
        float baseProductivity,
        List<Workspace> workspaces,
        List<WorkhorseAssignment> allAssignments)
    {
        if (self.Type != WorkhorseType.ToxicWolf)
            return baseProductivity;

        bool hasAdjacent = HasAdjacentWorkers(self, workspaces, allAssignments);
        return hasAdjacent ? CrowdedProductivity : AloneProductivity;
    }
}
