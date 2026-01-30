using System.Collections.Generic;

public class SynergySystem : ISynergyReference
{
    public static SynergySystem Instance { get; } = new();

    private readonly List<Synergy> _globalSynergies;
    private readonly List<Synergy> _adjacentSynergies;
    private readonly List<Synergy> _positionSynergies;
    private readonly List<Synergy> _abilitySynergies;
    private readonly List<Synergy> _allSynergies;

    public List<Synergy> GlobalSynergies => _globalSynergies;
    public List<Synergy> AdjacentSynergies => _adjacentSynergies;
    public List<Synergy> PositionSynergies => _positionSynergies;
    public List<Synergy> AbilitySynergies => _abilitySynergies;
    public List<Synergy> AllSynergies => _allSynergies;

    public SynergySystem()
    {
        // Old synergies disabled - no longer compatible with new workhorse types
        _globalSynergies = new List<Synergy>();
        _adjacentSynergies = new List<Synergy>();
        _positionSynergies = new List<Synergy>();

        // New ability synergies for the new workhorse types
        _abilitySynergies = new List<Synergy>
        {
            new ToxicWolfSynergy(),
            new EncouragerSynergy(),
            new RisingStarSynergy(),
            new FreeSpiritSynergy(),
            new PessimistSynergy(),
            new SaboteurSynergy()
        };

        _allSynergies = new List<Synergy>();
        _allSynergies.AddRange(_abilitySynergies);
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
