using System;
using UnityEngine;

public class PlayerProgress
{
    public static PlayerProgress Instance { get; } = new();

    private float _totalProductivity = 0f;
    private int _totalTurnsPlayed = 0;
    private int _maxSynergiesInOneTurn = 0;
    private int _currentDollar = GameSettings.StartingDollar;

    public float TotalProductivity => _totalProductivity;
    public int TotalTurnsPlayed => _totalTurnsPlayed;
    public int MaxSynergiesInOneTurn => _maxSynergiesInOneTurn;
    public int CurrentDollar => _currentDollar;

    // Events
    public event Action<float> OnProductivityChanged;
    public event Action<int> OnTurnsPlayedChanged;
    public event Action<int> OnDollarChanged;

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
        return _currentDollar >= amount;
    }

    public bool TrySpendDollar(int amount)
    {
        if (!CanAfford(amount))
            return false;

        _currentDollar -= amount;
        OnDollarChanged?.Invoke(_currentDollar);
        return true;
    }

    public void AddDollar(int amount)
    {
        if (amount <= 0)
            return;

        _currentDollar += amount;
        OnDollarChanged?.Invoke(_currentDollar);
    }

    public void Reset()
    {
        _totalProductivity = 0f;
        _totalTurnsPlayed = 0;
        _maxSynergiesInOneTurn = 0;
        _currentDollar = GameSettings.StartingDollar;

        OnProductivityChanged?.Invoke(_totalProductivity);
        OnTurnsPlayedChanged?.Invoke(_totalTurnsPlayed);
        OnDollarChanged?.Invoke(_currentDollar);
    }
}
