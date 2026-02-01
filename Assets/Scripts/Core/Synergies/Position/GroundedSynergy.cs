using System.Collections.Generic;
using System.Linq;

public class GroundedSynergy : Synergy
{
    public override string Name => "接地";
    public override SynergyType Type => SynergyType.Position;
    public override float BonusPercent => 10f;
    public override string Description => "劳动者位于底层";

    public override bool CheckActive(List<Workspace> workspaces, List<WorkhorseAssignment> assignments)
    {
        if (assignments.Count == 0) return false;

        var workspaceDict = workspaces.ToDictionary(w => w.Id);
        var occupiedWorkspaces = assignments
            .Where(a => workspaceDict.ContainsKey(a.WorkspaceId))
            .Select(a => workspaceDict[a.WorkspaceId])
            .ToList();

        // Bottom row is y == 1 (above ground level at y == 0)
        foreach (var w in occupiedWorkspaces)
        {
            if (w.Position.y == 1)
                return true;
        }

        return false;
    }
}
