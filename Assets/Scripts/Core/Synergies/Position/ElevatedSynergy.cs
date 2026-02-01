using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ElevatedSynergy : Synergy
{
    public override string Name => "高位";
    public override SynergyType Type => SynergyType.Position;
    public override float BonusPercent => 15f;
    public override string Description => "劳动者位于顶层";

    public override bool CheckActive(List<Workspace> workspaces, List<WorkhorseAssignment> assignments)
    {
        if (assignments.Count == 0 || workspaces.Count == 0) return false;

        var workspaceDict = workspaces.ToDictionary(w => w.Id);
        var occupiedWorkspaces = assignments
            .Where(a => workspaceDict.ContainsKey(a.WorkspaceId))
            .Select(a => workspaceDict[a.WorkspaceId])
            .ToList();

        if (occupiedWorkspaces.Count == 0) return false;

        // Find the highest Y position among all workspaces
        int maxY = 0;
        foreach (var w in workspaces)
        {
            maxY = Mathf.Max(maxY, w.Position.y + w.Size.y - 1);
        }

        // Check if any occupied workspace is at the top
        foreach (var w in occupiedWorkspaces)
        {
            if (w.Position.y + w.Size.y - 1 == maxY)
                return true;
        }

        return false;
    }
}
