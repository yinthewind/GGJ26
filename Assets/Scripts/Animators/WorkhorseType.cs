using System.Collections.Generic;

public enum WorkhorseType
{
    InternNiuma,
    RegularNiuma,
    SuperNiuma,
    ToxicWolf,
    Encourager,
    RisingStar,
    FreeSpirit,
    Pessimist,
    Saboteur,
}

public static class WorkhorsePrefabs
{
    private const string BasePath = "Addons/BasicPack/2_Prefab/Skelton/";

    private static readonly Dictionary<WorkhorseType, string> PrefabNames = new()
    {
        { WorkhorseType.InternNiuma, "SPUM_20240911215639833" },
        { WorkhorseType.RegularNiuma, "SPUM_20240911215639920" },
        { WorkhorseType.SuperNiuma, "SPUM_20240911215640266" },
        { WorkhorseType.ToxicWolf, "SPUM_20240911223046227" },
        { WorkhorseType.Encourager, "SPUM_20240911215640091" },
        { WorkhorseType.RisingStar, "SPUM_20240911215640179" },
        { WorkhorseType.FreeSpirit, "SPUM_20240911222954869" },
        { WorkhorseType.Pessimist, "SPUM_20240911222907638" },
        { WorkhorseType.Saboteur, "SPUM_20240911222823174" },
    };

    public static string GetPrefabPath(WorkhorseType type)
    {
        return BasePath + PrefabNames[type];
    }
}
