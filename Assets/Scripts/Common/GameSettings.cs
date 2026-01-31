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

    // Shop settings
    public const int StartingDollar = 50;
    public const int ShopRefreshCost = 10;
    public const int ShopRefreshCostIncrement = 10;
    public const int TotalShopSlots = 5;
    public const int InitialActiveSlots = 3;

    // Reveal settings
    public const int RevealCost = 15;
    public const int RevealCostIncrement = 5;

    // Workspace settings
    public const int WorkspacePrice = 20;

    // Fire cost (cost to dismiss a workhorse)
    public const int FireCost = 40;

    // Fixed price for all shop workhorses
    public const int ShopWorkhorsePrice = 20;

    // Productivity multipliers per workspace type
    public static readonly Dictionary<WorkspaceType, float> WorkspaceProductivityMultipliers = new()
    {
        { WorkspaceType.Basic, 1.0f },
        { WorkspaceType.Advanced, 1.5f },
        { WorkspaceType.Elite, 2.0f },
    };

    // Base productivity generation per worker type
    public static readonly Dictionary<WorkhorseType, float> WorkhorseProductivityRates = new()
    {
        { WorkhorseType.InternNiuma, 0.5f },     // 50 base
        { WorkhorseType.RegularNiuma, 1.0f },    // 100 base
        { WorkhorseType.SuperNiuma, 1.5f },      // 150 base
        { WorkhorseType.ToxicWolf, 2.0f },       // 200 base (modified by ability)
        { WorkhorseType.Encourager, 0.5f },      // 50 fixed
        { WorkhorseType.RisingStar, 0.5f },      // 50 starting (grows each round)
        { WorkhorseType.FreeSpirit, 1.0f },      // 100 fixed
        { WorkhorseType.Pessimist, 1.0f },       // 100 starting (decreases each round)
        { WorkhorseType.Saboteur, 0.0f },        // 0 income
    };

    // Purchase prices for each worker type (all same price)
    public static readonly Dictionary<WorkhorseType, int> WorkhorsePrices = new()
    {
        { WorkhorseType.InternNiuma, ShopWorkhorsePrice },
        { WorkhorseType.RegularNiuma, ShopWorkhorsePrice },
        { WorkhorseType.SuperNiuma, ShopWorkhorsePrice },
        { WorkhorseType.ToxicWolf, ShopWorkhorsePrice },
        { WorkhorseType.Encourager, ShopWorkhorsePrice },
        { WorkhorseType.RisingStar, ShopWorkhorsePrice },
        { WorkhorseType.FreeSpirit, ShopWorkhorsePrice },
        { WorkhorseType.Pessimist, ShopWorkhorsePrice },
        { WorkhorseType.Saboteur, ShopWorkhorsePrice },
    };

    // Placeholder colors for worker type icons (until sprites are added)
    public static readonly Dictionary<WorkhorseType, Color> WorkhorseColors = new()
    {
        { WorkhorseType.InternNiuma, new Color(0.7f, 0.8f, 0.7f) },  // Light green (junior)
        { WorkhorseType.RegularNiuma, new Color(0.5f, 0.7f, 0.5f) }, // Mid green
        { WorkhorseType.SuperNiuma, new Color(0.3f, 0.9f, 0.3f) },   // Bright green (senior)
        { WorkhorseType.ToxicWolf, new Color(0.5f, 0.8f, 0.2f) },    // Toxic green
        { WorkhorseType.Encourager, new Color(1.0f, 0.8f, 0.2f) },   // Golden yellow
        { WorkhorseType.RisingStar, new Color(1.0f, 0.6f, 0.0f) },   // Orange
        { WorkhorseType.FreeSpirit, new Color(0.6f, 0.8f, 1.0f) },   // Sky blue
        { WorkhorseType.Pessimist, new Color(0.4f, 0.4f, 0.5f) },    // Dark gray
        { WorkhorseType.Saboteur, new Color(0.2f, 0.2f, 0.2f) },     // Near black
    };

    public static float CalculateProductivity(WorkspaceType workspaceType, WorkhorseType workerType)
    {
        var workspaceMultiplier = WorkspaceProductivityMultipliers[workspaceType];
        var workerRate = WorkhorseProductivityRates[workerType];
        return workspaceMultiplier * workerRate;
    }

}
