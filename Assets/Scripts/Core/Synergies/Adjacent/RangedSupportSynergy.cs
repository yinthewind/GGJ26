using System.Collections.Generic;
using System.Linq;

public class RangedSupportSynergy : Synergy
{
    private static readonly HashSet<WorkhorseType> MeleeTypes = new()
    {
        WorkhorseType.Swordsman,
        WorkhorseType.Shieldbearer,
        WorkhorseType.Knight,
        WorkhorseType.DualBlade,
        WorkhorseType.Marauder,
        WorkhorseType.Viking,
        WorkhorseType.Berserker
    };

    public override string Name => "Ranged Support";
    public override SynergyType Type => SynergyType.Adjacent;
    public override float BonusPercent => 20f;
    public override string Description => "Archer adjacent to any melee";

    public override bool CheckActive(List<Workspace> workspaces, List<WorkhorseAssignment> assignments)
    {
        if (assignments.Count < 2) return false;

        var workspaceDict = workspaces.ToDictionary(w => w.Id);
        var archerAssignments = assignments.Where(a => a.Type == WorkhorseType.Archer).ToList();
        var meleeAssignments = assignments.Where(a => MeleeTypes.Contains(a.Type)).ToList();

        foreach (var archer in archerAssignments)
        {
            if (!workspaceDict.TryGetValue(archer.WorkspaceId, out var archerWorkspace)) continue;

            foreach (var melee in meleeAssignments)
            {
                if (!workspaceDict.TryGetValue(melee.WorkspaceId, out var meleeWorkspace)) continue;

                if (SynergyHelpers.AreWorkspacesAdjacent(archerWorkspace, meleeWorkspace))
                    return true;
            }
        }

        return false;
    }
}
