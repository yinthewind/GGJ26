using System.Collections.Generic;
using System.Linq;

public abstract class AdjacentPairSynergy : Synergy
{
    public override SynergyType Type => SynergyType.Adjacent;

    protected abstract WorkhorseType Type1 { get; }
    protected abstract WorkhorseType Type2 { get; }

    public override bool CheckActive(List<Workspace> workspaces, List<WorkhorseAssignment> assignments)
    {
        if (assignments.Count < 2) return false;

        var workspaceDict = workspaces.ToDictionary(w => w.Id);
        var type1Assignments = assignments.Where(a => a.Type == Type1).ToList();
        var type2Assignments = assignments.Where(a => a.Type == Type2).ToList();

        foreach (var a1 in type1Assignments)
        {
            if (!workspaceDict.TryGetValue(a1.WorkspaceId, out var w1)) continue;

            foreach (var a2 in type2Assignments)
            {
                if (!workspaceDict.TryGetValue(a2.WorkspaceId, out var w2)) continue;

                if (SynergyHelpers.AreWorkspacesAdjacent(w1, w2))
                    return true;
            }
        }

        return false;
    }
}
