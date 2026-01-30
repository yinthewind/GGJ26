using System.Collections.Generic;
using System.Linq;

public class MagicCouncilSynergy : Synergy
{
    private static readonly HashSet<WorkhorseType> MagicTypes = new()
    {
        WorkhorseType.Mage,
        WorkhorseType.BattleMage
    };

    public override string Name => "Magic Council";
    public override SynergyType Type => SynergyType.Global;
    public override float BonusPercent => 20f;
    public override string Description => "2+ magic users (Mage, BattleMage)";

    public override bool CheckActive(List<Workspace> workspaces, List<WorkhorseAssignment> assignments)
    {
        if (assignments.Count == 0) return false;
        var magicCount = assignments.Count(a => MagicTypes.Contains(a.Type));
        return magicCount >= 2;
    }
}
