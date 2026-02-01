using System.Collections.Generic;

/// <summary>
/// FreeSpirit (自由派): -30% income to adjacent workers.
/// Distracts nearby teammates with their carefree attitude.
/// </summary>
public class FreeSpiritSynergy : AbilitySynergy
{
    public override string Name => "自由派";
    public override WorkhorseType WorkhorseType => WorkhorseType.FreeSpirit;
    public override float BonusPercent => 0f;
    public override string Description => "相邻劳动者收入-30%";

    private const float AdjacentPenalty = 0.7f;  // -30%

    public override float GetAdjacentMultiplier(
        WorkhorseAssignment self,
        WorkhorseAssignment adjacent,
        List<Workspace> workspaces)
    {
        if (self.Type != WorkhorseType.FreeSpirit)
            return 1.0f;

        if (AreWorkersAdjacent(self, adjacent, workspaces))
            return AdjacentPenalty;

        return 1.0f;
    }
}
