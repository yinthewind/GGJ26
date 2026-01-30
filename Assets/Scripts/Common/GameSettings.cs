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
    public static readonly Dictionary<SkeletonType, float> SkeletonManaRates = new()
    {
        { SkeletonType.Swordsman, 1.0f },
        { SkeletonType.Archer, 1.0f },
        { SkeletonType.Shieldbearer, 1.0f },
        { SkeletonType.Mage, 2.0f },
        { SkeletonType.BattleMage, 2.5f },
        { SkeletonType.Knight, 1.2f },
        { SkeletonType.DualBlade, 1.3f },
        { SkeletonType.Marauder, 1.1f },
        { SkeletonType.Viking, 1.1f },
        { SkeletonType.Berserker, 1.5f },
    };

    // Purchase prices for each skeleton type
    public static readonly Dictionary<SkeletonType, int> SkeletonPrices = new()
    {
        { SkeletonType.Swordsman, 10 },
        { SkeletonType.Archer, 10 },
        { SkeletonType.Shieldbearer, 15 },
        { SkeletonType.Mage, 25 },
        { SkeletonType.BattleMage, 35 },
        { SkeletonType.Knight, 20 },
        { SkeletonType.DualBlade, 18 },
        { SkeletonType.Marauder, 22 },
        { SkeletonType.Viking, 20 },
        { SkeletonType.Berserker, 30 },
    };

    // Placeholder colors for skeleton type icons (until sprites are added)
    public static readonly Dictionary<SkeletonType, Color> SkeletonColors = new()
    {
        { SkeletonType.Swordsman, new Color(0.6f, 0.6f, 0.7f) },    // Steel gray
        { SkeletonType.Archer, new Color(0.4f, 0.7f, 0.4f) },       // Forest green
        { SkeletonType.Shieldbearer, new Color(0.5f, 0.5f, 0.6f) }, // Shield gray
        { SkeletonType.Mage, new Color(0.6f, 0.4f, 0.8f) },         // Purple
        { SkeletonType.BattleMage, new Color(0.8f, 0.3f, 0.6f) },   // Magenta
        { SkeletonType.Knight, new Color(0.7f, 0.7f, 0.5f) },       // Golden
        { SkeletonType.DualBlade, new Color(0.4f, 0.6f, 0.7f) },    // Blue steel
        { SkeletonType.Marauder, new Color(0.7f, 0.5f, 0.3f) },     // Bronze
        { SkeletonType.Viking, new Color(0.5f, 0.6f, 0.8f) },       // Ice blue
        { SkeletonType.Berserker, new Color(0.8f, 0.3f, 0.3f) },    // Blood red
    };

    public static float CalculateMana(WorkspaceType workspaceType, SkeletonType skeletonType)
    {
        var workspaceMultiplier = WorkspaceManaMultipliers[workspaceType];
        var skeletonRate = SkeletonManaRates[skeletonType];
        return workspaceMultiplier * skeletonRate;
    }
}
