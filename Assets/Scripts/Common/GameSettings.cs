using System.Collections.Generic;
using UnityEngine;

public enum WorkspaceType
{
    Basic,
    Advanced,
    Elite,
}

public static class GameSettings
{
    public static float WorldWidth = 10f;  // Total world width
    public static float ManaGenerationInterval = 2.0f;  // Seconds between mana generation ticks

    // Mana multipliers per workspace type
    public static readonly Dictionary<WorkspaceType, float> WorkspaceManaMultipliers = new()
    {
        { WorkspaceType.Basic, 1.0f },
        { WorkspaceType.Advanced, 1.5f },
        { WorkspaceType.Elite, 2.0f },
    };

    // Base mana generation per skeleton type
    public static readonly Dictionary<WorkhorseType, float> WorkhorseManaRates = new()
    {
        { WorkhorseType.Swordsman, 1.0f },
        { WorkhorseType.Archer, 1.0f },
        { WorkhorseType.Shieldbearer, 1.0f },
        { WorkhorseType.Mage, 2.0f },
        { WorkhorseType.BattleMage, 2.5f },
        { WorkhorseType.Knight, 1.2f },
        { WorkhorseType.DualBlade, 1.3f },
        { WorkhorseType.Marauder, 1.1f },
        { WorkhorseType.Viking, 1.1f },
        { WorkhorseType.Berserker, 1.5f },
    };

    // Purchase prices for each skeleton type
    public static readonly Dictionary<WorkhorseType, int> WorkhorsePrices = new()
    {
        { WorkhorseType.Swordsman, 10 },
        { WorkhorseType.Archer, 10 },
        { WorkhorseType.Shieldbearer, 15 },
        { WorkhorseType.Mage, 25 },
        { WorkhorseType.BattleMage, 35 },
        { WorkhorseType.Knight, 20 },
        { WorkhorseType.DualBlade, 18 },
        { WorkhorseType.Marauder, 22 },
        { WorkhorseType.Viking, 20 },
        { WorkhorseType.Berserker, 30 },
    };

    // Placeholder colors for skeleton type icons (until sprites are added)
    public static readonly Dictionary<WorkhorseType, Color> WorkhorseColors = new()
    {
        { WorkhorseType.Swordsman, new Color(0.6f, 0.6f, 0.7f) },    // Steel gray
        { WorkhorseType.Archer, new Color(0.4f, 0.7f, 0.4f) },       // Forest green
        { WorkhorseType.Shieldbearer, new Color(0.5f, 0.5f, 0.6f) }, // Shield gray
        { WorkhorseType.Mage, new Color(0.6f, 0.4f, 0.8f) },         // Purple
        { WorkhorseType.BattleMage, new Color(0.8f, 0.3f, 0.6f) },   // Magenta
        { WorkhorseType.Knight, new Color(0.7f, 0.7f, 0.5f) },       // Golden
        { WorkhorseType.DualBlade, new Color(0.4f, 0.6f, 0.7f) },    // Blue steel
        { WorkhorseType.Marauder, new Color(0.7f, 0.5f, 0.3f) },     // Bronze
        { WorkhorseType.Viking, new Color(0.5f, 0.6f, 0.8f) },       // Ice blue
        { WorkhorseType.Berserker, new Color(0.8f, 0.3f, 0.3f) },    // Blood red
    };

    public static float CalculateMana(WorkspaceType workspaceType, WorkhorseType skeletonType)
    {
        var workspaceMultiplier = WorkspaceManaMultipliers[workspaceType];
        var skeletonRate = WorkhorseManaRates[skeletonType];
        return workspaceMultiplier * skeletonRate;
    }
}
