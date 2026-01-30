using System.Collections.Generic;

public enum SkeletonType
{
    Swordsman,
    Archer,
    Shieldbearer,
    Mage,
    BattleMage,
    Knight,
    DualBlade,
    Marauder,
    Viking,
    Berserker,
}

public static class SkeletonPrefabs
{
    private const string BasePath = "Addons/BasicPack/2_Prefab/Skelton/";

    private static readonly Dictionary<SkeletonType, string> PrefabNames = new()
    {
        { SkeletonType.Swordsman, "SPUM_20240911215639833" },
        { SkeletonType.Archer, "SPUM_20240911215639920" },
        { SkeletonType.Shieldbearer, "SPUM_20240911215640005" },
        { SkeletonType.Mage, "SPUM_20240911215640091" },
        { SkeletonType.BattleMage, "SPUM_20240911215640179" },
        { SkeletonType.Knight, "SPUM_20240911215640266" },
        { SkeletonType.DualBlade, "SPUM_20240911222823174" },
        { SkeletonType.Marauder, "SPUM_20240911222907638" },
        { SkeletonType.Viking, "SPUM_20240911222954869" },
        { SkeletonType.Berserker, "SPUM_20240911223046227" },
    };

    public static string GetPrefabPath(SkeletonType type)
    {
        return BasePath + PrefabNames[type];
    }
}
