using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#region Global Synergies

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

#endregion

#region Adjacent Synergies

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

public class FrontlineDuoSynergy : AdjacentPairSynergy
{
    public override string Name => "Frontline Duo";
    public override float BonusPercent => 25f;
    public override string Description => "Swordsman + Shieldbearer adjacent";

    protected override WorkhorseType Type1 => WorkhorseType.Swordsman;
    protected override WorkhorseType Type2 => WorkhorseType.Shieldbearer;
}

public class RangedSupportSynergy : Synergy
{
    private static readonly HashSet<WorkhorseType> MeleeTypes = new()
    {
        WorkhorseType.Swordsman,
        WorkhorseType.Shieldbearer,
        WorkhorseType.Knight,
        WorkhorseType.DualBlade,
        WorkhorseType.Marauder,
        WorkhorseType.Viking,
        WorkhorseType.Berserker
    };

    public override string Name => "Ranged Support";
    public override SynergyType Type => SynergyType.Adjacent;
    public override float BonusPercent => 20f;
    public override string Description => "Archer adjacent to any melee";

    public override bool CheckActive(List<Workspace> workspaces, List<WorkhorseAssignment> assignments)
    {
        if (assignments.Count < 2) return false;

        var workspaceDict = workspaces.ToDictionary(w => w.Id);
        var archerAssignments = assignments.Where(a => a.Type == WorkhorseType.Archer).ToList();
        var meleeAssignments = assignments.Where(a => MeleeTypes.Contains(a.Type)).ToList();

        foreach (var archer in archerAssignments)
        {
            if (!workspaceDict.TryGetValue(archer.WorkspaceId, out var archerWorkspace)) continue;

            foreach (var melee in meleeAssignments)
            {
                if (!workspaceDict.TryGetValue(melee.WorkspaceId, out var meleeWorkspace)) continue;

                if (SynergyHelpers.AreWorkspacesAdjacent(archerWorkspace, meleeWorkspace))
                    return true;
            }
        }

        return false;
    }
}

public class BattleBrothersSynergy : AdjacentPairSynergy
{
    public override string Name => "Battle Brothers";
    public override float BonusPercent => 30f;
    public override string Description => "Viking + Berserker adjacent";

    protected override WorkhorseType Type1 => WorkhorseType.Viking;
    protected override WorkhorseType Type2 => WorkhorseType.Berserker;
}

public class BladeMastersSynergy : AdjacentPairSynergy
{
    public override string Name => "Blade Masters";
    public override float BonusPercent => 20f;
    public override string Description => "DualBlade + Knight adjacent";

    protected override WorkhorseType Type1 => WorkhorseType.DualBlade;
    protected override WorkhorseType Type2 => WorkhorseType.Knight;
}

#endregion

#region Position Synergies

public class CenteredSynergy : Synergy
{
    public override string Name => "Centered";
    public override SynergyType Type => SynergyType.Position;
    public override float BonusPercent => 15f;
    public override string Description => "Worker near geometric center";

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

public class GroundedSynergy : Synergy
{
    public override string Name => "Grounded";
    public override SynergyType Type => SynergyType.Position;
    public override float BonusPercent => 10f;
    public override string Description => "Worker on bottom row";

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

public class ElevatedSynergy : Synergy
{
    public override string Name => "Elevated";
    public override SynergyType Type => SynergyType.Position;
    public override float BonusPercent => 15f;
    public override string Description => "Worker on top of stack";

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

#endregion

#region Helpers

public static class SynergyHelpers
{
    public static bool AreWorkspacesAdjacent(Workspace w1, Workspace w2)
    {
        var cells1 = GetOccupiedCells(w1);
        var cells2 = GetOccupiedCells(w2);

        foreach (var c1 in cells1)
        {
            foreach (var c2 in cells2)
            {
                if (AreCellsAdjacent(c1, c2))
                    return true;
            }
        }

        return false;
    }

    public static List<Vector2Int> GetOccupiedCells(Workspace workspace)
    {
        var cells = new List<Vector2Int>();
        for (int x = 0; x < workspace.Size.x; x++)
        {
            for (int y = 0; y < workspace.Size.y; y++)
            {
                cells.Add(new Vector2Int(workspace.Position.x + x, workspace.Position.y + y));
            }
        }
        return cells;
    }

    public static bool AreCellsAdjacent(Vector2Int c1, Vector2Int c2)
    {
        int dx = Mathf.Abs(c1.x - c2.x);
        int dy = Mathf.Abs(c1.y - c2.y);
        return (dx == 1 && dy == 0) || (dx == 0 && dy == 1);
    }
}

#endregion

public class SynergySystem : ISynergyReference
{
    public static SynergySystem Instance { get; } = new();

    private readonly List<Synergy> _globalSynergies;
    private readonly List<Synergy> _adjacentSynergies;
    private readonly List<Synergy> _positionSynergies;
    private readonly List<Synergy> _allSynergies;

    public List<Synergy> GlobalSynergies => _globalSynergies;
    public List<Synergy> AdjacentSynergies => _adjacentSynergies;
    public List<Synergy> PositionSynergies => _positionSynergies;
    public List<Synergy> AllSynergies => _allSynergies;

    public SynergySystem()
    {
        _globalSynergies = new List<Synergy>
        {
            new DiverseTeamSynergy(),
            new FullHouseSynergy(),
            new MagicCouncilSynergy()
        };

        _adjacentSynergies = new List<Synergy>
        {
            new FrontlineDuoSynergy(),
            new RangedSupportSynergy(),
            new BattleBrothersSynergy(),
            new BladeMastersSynergy()
        };

        _positionSynergies = new List<Synergy>
        {
            new CenteredSynergy(),
            new CorneredSynergy(),
            new GroundedSynergy(),
            new ElevatedSynergy()
        };

        _allSynergies = new List<Synergy>();
        _allSynergies.AddRange(_globalSynergies);
        _allSynergies.AddRange(_adjacentSynergies);
        _allSynergies.AddRange(_positionSynergies);
    }

    public List<SynergyResult> GetActiveSynergies(List<Workspace> workspaces, List<WorkhorseAssignment> assignments)
    {
        var results = new List<SynergyResult>();

        foreach (var synergy in _allSynergies)
        {
            results.Add(new SynergyResult
            {
                Synergy = synergy,
                IsActive = synergy.CheckActive(workspaces, assignments)
            });
        }

        return results;
    }
}
