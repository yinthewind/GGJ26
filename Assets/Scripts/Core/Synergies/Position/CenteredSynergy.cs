using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CenteredSynergy : Synergy
{
    public override string Name => "中心";
    public override SynergyType Type => SynergyType.Position;
    public override float BonusPercent => 15f;
    public override string Description => "劳动者位于几何中心附近";

    public override bool CheckActive(List<Workspace> workspaces, List<WorkhorseAssignment> assignments)
    {
        if (assignments.Count == 0 || workspaces.Count == 0) return false;

        var workspaceDict = workspaces.ToDictionary(w => w.Id);
        var occupiedWorkspaces = assignments
            .Where(a => workspaceDict.ContainsKey(a.WorkspaceId))
            .Select(a => workspaceDict[a.WorkspaceId])
            .ToList();

        if (occupiedWorkspaces.Count == 0) return false;

        // Calculate geometric center of all workspaces
        float centerX = 0f;
        float centerY = 0f;
        foreach (var w in workspaces)
        {
            centerX += w.Position.x + w.Size.x / 2f;
            centerY += w.Position.y + w.Size.y / 2f;
        }
        centerX /= workspaces.Count;
        centerY /= workspaces.Count;

        // Check if any occupied workspace is within 1 unit of center
        foreach (var w in occupiedWorkspaces)
        {
            float wx = w.Position.x + w.Size.x / 2f;
            float wy = w.Position.y + w.Size.y / 2f;
            float distance = Mathf.Sqrt((wx - centerX) * (wx - centerX) + (wy - centerY) * (wy - centerY));
            if (distance <= 1f)
                return true;
        }

        return false;
    }
}
