public class BladeMastersSynergy : AdjacentPairSynergy
{
    public override string Name => "Blade Masters";
    public override float BonusPercent => 20f;
    public override string Description => "DualBlade + Knight adjacent";

    protected override WorkhorseType Type1 => WorkhorseType.DualBlade;
    protected override WorkhorseType Type2 => WorkhorseType.Knight;
}
