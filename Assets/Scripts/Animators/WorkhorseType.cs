using System.Collections.Generic;

public enum WorkhorseType
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

public static class WorkhorsePrefabs
{
    private const string BasePath = "Addons/BasicPack/2_Prefab/Skelton/";

    private static readonly Dictionary<WorkhorseType, string> PrefabNames = new()
    {
        { WorkhorseType.Swordsman, "SPUM_20240911215639833" },
        { WorkhorseType.Archer, "SPUM_20240911215639920" },
        { WorkhorseType.Shieldbearer, "SPUM_20240911215640005" },
        { WorkhorseType.Mage, "SPUM_20240911215640091" },
        { WorkhorseType.BattleMage, "SPUM_20240911215640179" },
        { WorkhorseType.Knight, "SPUM_20240911215640266" },
        { WorkhorseType.DualBlade, "SPUM_20240911222823174" },
        { WorkhorseType.Marauder, "SPUM_20240911222907638" },
        { WorkhorseType.Viking, "SPUM_20240911222954869" },
        { WorkhorseType.Berserker, "SPUM_20240911223046227" },
    };

    public static string GetPrefabPath(WorkhorseType type)
    {
        return BasePath + PrefabNames[type];
    }
}
