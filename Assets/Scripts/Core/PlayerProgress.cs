using System;
using UnityEngine;

public class PlayerProgress
{
    public static PlayerProgress Instance { get; } = new();

    private float _totalProductivity = 0f;
    private int _totalTurnsPlayed = 0;
    private int _maxSynergiesInOneTurn = 0;

    public float TotalProductivity => _totalProductivity;
    public int TotalTurnsPlayed => _totalTurnsPlayed;
    public int MaxSynergiesInOneTurn => _maxSynergiesInOneTurn;

    // Events
    public event Action<float> OnProductivityChanged;
    public event Action<int> OnTurnsPlayedChanged;

    public void AddProductivity(float amount)
    {
        if (amount <= 0)
            return;

        _totalProductivity += amount;
        _totalTurnsPlayed++;

        OnProductivityChanged?.Invoke(_totalProductivity);
        OnTurnsPlayedChanged?.Invoke(_totalTurnsPlayed);
    }

    public void UpdateMaxSynergies(int synergiesThisTurn)
    {
        if (synergiesThisTurn > _maxSynergiesInOneTurn)
        {
            _maxSynergiesInOneTurn = synergiesThisTurn;
        }
    }

    public void Reset()
    {
        _totalProductivity = 0f;
        _totalTurnsPlayed = 0;
        _maxSynergiesInOneTurn = 0;

        OnProductivityChanged?.Invoke(_totalProductivity);
        OnTurnsPlayedChanged?.Invoke(_totalTurnsPlayed);
    }
}
