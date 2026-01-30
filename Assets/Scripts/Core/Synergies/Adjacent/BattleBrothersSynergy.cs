public class BattleBrothersSynergy : AdjacentPairSynergy
{
    public override string Name => "Battle Brothers";
    public override float BonusPercent => 30f;
    public override string Description => "Viking + Berserker adjacent";

    protected override WorkhorseType Type1 => WorkhorseType.Viking;
    protected override WorkhorseType Type2 => WorkhorseType.Berserker;
}
