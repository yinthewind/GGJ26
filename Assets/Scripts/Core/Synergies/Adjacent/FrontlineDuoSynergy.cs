public class FrontlineDuoSynergy : AdjacentPairSynergy
{
    public override string Name => "Frontline Duo";
    public override float BonusPercent => 25f;
    public override string Description => "Swordsman + Shieldbearer adjacent";

    protected override WorkhorseType Type1 => WorkhorseType.Swordsman;
    protected override WorkhorseType Type2 => WorkhorseType.Shieldbearer;
}
