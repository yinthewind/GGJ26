using System.Collections.Generic;

public class LevelConfig
{
    public string LevelId;
    public string LevelName;
    public List<WorkhorseType> AvailableWorkhorseTypes;
    public int TurnLimit;

    // Goal fields
    public string GoalName;
    public string GoalDescription;
    public GoalType GoalType;
    public float GoalTargetValue;

    // Reward for completing the project
    public int ProjectReward;

    public LevelConfig(
        string levelId,
        string levelName,
        List<WorkhorseType> availableWorkhorseTypes,
        int turnLimit,
        string goalName,
        string goalDescription,
        GoalType goalType,
        float goalTargetValue,
        int projectReward)
    {
        LevelId = levelId;
        LevelName = levelName;
        AvailableWorkhorseTypes = availableWorkhorseTypes;
        TurnLimit = turnLimit;
        GoalName = goalName;
        GoalDescription = goalDescription;
        GoalType = goalType;
        GoalTargetValue = goalTargetValue;
        ProjectReward = projectReward;
    }
}
