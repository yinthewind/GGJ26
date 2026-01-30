using System;
using UnityEngine;

public class PlayerProgress
{
    public static PlayerProgress Instance { get; } = new();

    private float _totalProductivity = 0f;
    private int _totalTurnsPlayed = 0;
    private int _maxSynergiesInOneTurn = 0;
    private int _currentGold = GameSettings.StartingGold;

    public float TotalProductivity => _totalProductivity;
    public int TotalTurnsPlayed => _totalTurnsPlayed;
    public int MaxSynergiesInOneTurn => _maxSynergiesInOneTurn;
    public int CurrentGold => _currentGold;

    // Events
    public event Action<float> OnProductivityChanged;
    public event Action<int> OnTurnsPlayedChanged;
    public event Action<int> OnGoldChanged;

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

    public bool CanAfford(int amount)
    {
        return _currentGold >= amount;
    }

    public bool TrySpendGold(int amount)
    {
        if (!CanAfford(amount))
            return false;

        _currentGold -= amount;
        OnGoldChanged?.Invoke(_currentGold);
        return true;
    }

    public void AddGold(int amount)
    {
        if (amount <= 0)
            return;

        _currentGold += amount;
        OnGoldChanged?.Invoke(_currentGold);
    }

    public void Reset()
    {
        _totalProductivity = 0f;
        _totalTurnsPlayed = 0;
        _maxSynergiesInOneTurn = 0;
        _currentGold = GameSettings.StartingGold;

        OnProductivityChanged?.Invoke(_totalProductivity);
        OnTurnsPlayedChanged?.Invoke(_totalTurnsPlayed);
        OnGoldChanged?.Invoke(_currentGold);
    }
}
