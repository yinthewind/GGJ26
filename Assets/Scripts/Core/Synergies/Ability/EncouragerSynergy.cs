using System.Collections.Generic;

/// <summary>
/// Encourager (鼓励者): +50% income to adjacent workers.
/// Boosts nearby teammates' productivity.
/// </summary>
public class EncouragerSynergy : AbilitySynergy
{
    public override string Name => "Encourager";
    public override WorkhorseType WorkhorseType => WorkhorseType.Encourager;
    public override float BonusPercent => 0f;
    public override string Description => "+50% income to adjacent workers";

    private const float AdjacentBonus = 1.5f;  // +50%

    public override float GetAdjacentMultiplier(
        WorkhorseAssignment self,
        WorkhorseAssignment adjacent,
        List<Workspace> workspaces)
    {
        if (self.Type != WorkhorseType.Encourager)
            return 1.0f;

        if (AreWorkersAdjacent(self, adjacent, workspaces))
            return AdjacentBonus;

        return 1.0f;
    }
}
