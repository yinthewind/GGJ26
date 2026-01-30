using System.Collections.Generic;

public class FullHouseSynergy : Synergy
{
    public override string Name => "Full House";
    public override SynergyType Type => SynergyType.Global;
    public override float BonusPercent => 10f;
    public override string Description => "5+ workers assigned";

    public override bool CheckActive(List<Workspace> workspaces, List<WorkhorseAssignment> assignments)
    {
        return assignments.Count >= 5;
    }
}
