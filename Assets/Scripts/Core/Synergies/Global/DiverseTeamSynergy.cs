using System.Collections.Generic;
using System.Linq;

public class DiverseTeamSynergy : Synergy
{
    public override string Name => "Diverse Team";
    public override SynergyType Type => SynergyType.Global;
    public override float BonusPercent => 15f;
    public override string Description => "3+ different worker types";

    public override bool CheckActive(List<Workspace> workspaces, List<WorkhorseAssignment> assignments)
    {
        if (assignments.Count == 0) return false;
        var uniqueTypes = assignments.Select(a => a.Type).Distinct().Count();
        return uniqueTypes >= 3;
    }
}
