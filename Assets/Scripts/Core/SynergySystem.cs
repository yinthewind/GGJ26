using System.Collections.Generic;

public class SynergySystem : ISynergyReference
{
    public static SynergySystem Instance { get; } = new();

    private readonly List<Synergy> _globalSynergies;
    private readonly List<Synergy> _adjacentSynergies;
    private readonly List<Synergy> _positionSynergies;
    private readonly List<Synergy> _allSynergies;

    public List<Synergy> GlobalSynergies => _globalSynergies;
    public List<Synergy> AdjacentSynergies => _adjacentSynergies;
    public List<Synergy> PositionSynergies => _positionSynergies;
    public List<Synergy> AllSynergies => _allSynergies;

    public SynergySystem()
    {
        _globalSynergies = new List<Synergy>
        {
            new DiverseTeamSynergy(),
            new FullHouseSynergy(),
            new MagicCouncilSynergy()
        };

        _adjacentSynergies = new List<Synergy>
        {
            new FrontlineDuoSynergy(),
            new RangedSupportSynergy(),
            new BattleBrothersSynergy(),
            new BladeMastersSynergy()
        };

        _positionSynergies = new List<Synergy>
        {
            new CenteredSynergy(),
            new CorneredSynergy(),
            new GroundedSynergy(),
            new ElevatedSynergy()
        };

        _allSynergies = new List<Synergy>();
        _allSynergies.AddRange(_globalSynergies);
        _allSynergies.AddRange(_adjacentSynergies);
        _allSynergies.AddRange(_positionSynergies);
    }

    public List<SynergyResult> GetActiveSynergies(List<Workspace> workspaces, List<WorkhorseAssignment> assignments)
    {
        var results = new List<SynergyResult>();

        foreach (var synergy in _allSynergies)
        {
            results.Add(new SynergyResult
            {
                Synergy = synergy,
                IsActive = synergy.CheckActive(workspaces, assignments)
            });
        }

        return results;
    }
}
