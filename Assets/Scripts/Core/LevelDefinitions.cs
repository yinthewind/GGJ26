using System.Collections.Generic;

public static class LevelDefinitions
{
    public static readonly Dictionary<string, LevelConfig> Levels = new()
    {
        {
            "level_1",
            new LevelConfig(
                levelId: "level_1",
                levelName: "Getting Started",
                availableWorkhorseTypes: new List<WorkhorseType>
                {
                    WorkhorseType.InternNiuma,
                    WorkhorseType.RegularNiuma,
                    WorkhorseType.ToxicWolf,
                    WorkhorseType.Encourager
                },
                turnLimit: 3,
                goalName: "First Steps",
                goalType: GoalType.TotalProductivity,
                goalTargetValue: 4,
                projectReward: 100
            )
        },
        {
            "level_2",
            new LevelConfig(
                levelId: "level_2",
                levelName: "Team Building",
                availableWorkhorseTypes: new List<WorkhorseType>
                {
                    WorkhorseType.InternNiuma,
                    WorkhorseType.RegularNiuma,
                    WorkhorseType.ToxicWolf,
                    WorkhorseType.SuperNiuma,
                    WorkhorseType.Encourager
                },
                turnLimit: 5,
                goalName: "Team Building",
                goalType: GoalType.TotalProductivity,
                goalTargetValue: 20,
                projectReward: 200
            )
        },
        {
            "level_3",
            new LevelConfig(
                levelId: "level_3",
                levelName: "Office Politics",
                availableWorkhorseTypes: new List<WorkhorseType>
                {
                    WorkhorseType.InternNiuma,
                    WorkhorseType.RegularNiuma,
                    WorkhorseType.SuperNiuma,
                    WorkhorseType.ToxicWolf,
                    WorkhorseType.Encourager,
                    WorkhorseType.Pessimist
                },
                turnLimit: 8,
                goalName: "Office Politics",
                goalType: GoalType.TotalProductivity,
                goalTargetValue: 40,
                projectReward: 200
            )
        },
        {
            "level_4",
            new LevelConfig(
                levelId: "level_4",
                levelName: "Chaos Management",
                availableWorkhorseTypes: new List<WorkhorseType>
                {
                    WorkhorseType.InternNiuma,
                    WorkhorseType.RegularNiuma,
                    WorkhorseType.SuperNiuma,
                    WorkhorseType.ToxicWolf,
                    WorkhorseType.Encourager,
                    WorkhorseType.RisingStar,
                    WorkhorseType.FreeSpirit,
                    WorkhorseType.Pessimist,
                    WorkhorseType.Saboteur
                },
                turnLimit: 5,
                goalName: "Chaos Management",
                goalType: GoalType.TotalProductivity,
                goalTargetValue: 20f,
                projectReward: 150
            )
        }
    };

    public static string FirstLevelId => "level_1";

    public static LevelConfig GetLevel(string levelId)
    {
        if (Levels.TryGetValue(levelId, out var config))
        {
            return config;
        }
        return null;
    }

    public static string GetNextLevelId(string currentLevelId)
    {
        return currentLevelId switch
        {
            "level_1" => "level_2",
            "level_2" => "level_3",
            "level_3" => "level_4",
            _ => null
        };
    }
}
