using System.Collections.Generic;

public class FullHouseSynergy : Synergy
{
    public override string Name => "满员";
    public override SynergyType Type => SynergyType.Global;
    public override float BonusPercent => 10f;
    public override string Description => "分配5名以上劳动者";

    public override bool CheckActive(List<Workspace> workspaces, List<WorkhorseAssignment> assignments)
    {
        return assignments.Count >= 5;
    }
}
