using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CorneredSynergy : Synergy
{
    public override string Name => "Cornered";
    public override SynergyType Type => SynergyType.Position;
    public override float BonusPercent => 20f;
    public override string Description => "Worker in corner (2+ wall edges)";

    public override bool CheckActive(List<Workspace> workspaces, List<WorkhorseAssignment> assignments)
    {
        if (assignments.Count == 0 || workspaces.Count == 0) return false;

        var workspaceDict = workspaces.ToDictionary(w => w.Id);
        var occupiedWorkspaces = assignments
            .Where(a => workspaceDict.ContainsKey(a.WorkspaceId))
            .Select(a => workspaceDict[a.WorkspaceId])
            .ToList();

        if (occupiedWorkspaces.Count == 0) return false;

        // Find bounds of all workspaces
        int minX = int.MaxValue, maxX = int.MinValue;
        int minY = int.MaxValue, maxY = int.MinValue;

        foreach (var w in workspaces)
        {
            minX = Mathf.Min(minX, w.Position.x);
            maxX = Mathf.Max(maxX, w.Position.x + w.Size.x - 1);
            minY = Mathf.Min(minY, w.Position.y);
            maxY = Mathf.Max(maxY, w.Position.y + w.Size.y - 1);
        }

        // Check if any occupied workspace is at a corner
        foreach (var w in occupiedWorkspaces)
        {
            int edgeCount = 0;
            if (w.Position.x == minX) edgeCount++;
            if (w.Position.x + w.Size.x - 1 == maxX) edgeCount++;
            if (w.Position.y == minY) edgeCount++;
            if (w.Position.y + w.Size.y - 1 == maxY) edgeCount++;

            if (edgeCount >= 2)
                return true;
        }

        return false;
    }
}
