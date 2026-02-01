using System.Collections.Generic;
using System.Linq;

public class DiverseTeamSynergy : Synergy
{
    public override string Name => "多元团队";
    public override SynergyType Type => SynergyType.Global;
    public override float BonusPercent => 15f;
    public override string Description => "3种以上不同劳动者";

    public override bool CheckActive(List<Workspace> workspaces, List<WorkhorseAssignment> assignments)
    {
        if (assignments.Count == 0) return false;
        var uniqueTypes = assignments.Select(a => a.Type).Distinct().Count();
        return uniqueTypes >= 3;
    }
}
