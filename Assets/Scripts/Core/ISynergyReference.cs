using System.Collections.Generic;
using UnityEngine;

public enum SynergyType
{
    Global,
    Adjacent,
    Position
}

public struct SynergyDefinition
{
    public string Name;
    public SynergyType Type;
    public float BonusPercent;
    public string Description;
}

public struct SynergyResult
{
    public Synergy Synergy;
    public bool IsActive;
}

/// <summary>
/// Abstract base class for all synergies. Each synergy encapsulates its own activation logic.
/// </summary>
public abstract class Synergy
{
    public abstract string Name { get; }
    public abstract SynergyType Type { get; }
    public abstract float BonusPercent { get; }
    public abstract string Description { get; }

    /// <summary>
    /// Check if this synergy is currently active given the workspace layout and worker assignments.
    /// </summary>
    public abstract bool CheckActive(List<Workspace> workspaces, List<WorkhorseAssignment> assignments);
}

/// <summary>
/// Lightweight workspace data for productivity calculation.
/// </summary>
public struct Workspace
{
    public int Id;
    public WorkspaceType Type;
    public Vector2Int Position;
    public Vector2Int Size;
}

/// <summary>
/// Worker-to-workspace assignment for productivity calculation.
/// </summary>
public struct WorkhorseAssignment
{
    public int WorkerId;
    public int WorkspaceId;
    public WorkhorseType Type;
}

public interface ISynergyReference
{
    List<Synergy> GlobalSynergies { get; }
    List<Synergy> AdjacentSynergies { get; }
    List<Synergy> PositionSynergies { get; }
    List<Synergy> AllSynergies { get; }
}
