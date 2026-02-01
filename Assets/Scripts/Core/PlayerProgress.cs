using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public struct SavedWorkhorseData
{
    public Guid EntityId;
    public WorkhorseType Type;
    public Guid? AssignedWorkspaceId;
    public Vector3 Position; // Used only for unassigned workhorses
    public bool IsRevealed;
    public int CharacterIndex; // Persisted sprite index for visual consistency across levels
}

public struct SavedWorkspaceData
{
    public Guid EntityId;
    public Vector2Int GridPosition;
}

public class PlayerProgress
{
    public static PlayerProgress Instance { get; } = new();

    private float _totalProductivity = 0f;
    private int _totalTurnsPlayed = 0;
    private int _maxSynergiesInOneTurn = 0;
    private int _currentDollar = GameSettings.StartingDollar;
    private List<SavedWorkspaceData> _workspaceData = new();
    private List<SavedWorkhorseData> _workhorseData = new();

    public float TotalProductivity => _totalProductivity;
    public int TotalTurnsPlayed => _totalTurnsPlayed;
    public int MaxSynergiesInOneTurn => _maxSynergiesInOneTurn;
    public int CurrentDollar => _currentDollar;
    public IReadOnlyList<SavedWorkspaceData> WorkspaceData => _workspaceData;
    public IReadOnlyList<SavedWorkhorseData> WorkhorseData => _workhorseData;

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

    public void SaveGameState(IReadOnlyList<WorkspaceController> workspaces, IReadOnlyList<WorkhorseController> workhorses)
    {
        // Save workspace data with GUIDs
        _workspaceData.Clear();
        foreach (var workspace in workspaces)
        {
            _workspaceData.Add(new SavedWorkspaceData
            {
                EntityId = workspace.EntityId,
                GridPosition = workspace.GridPosition
            });
        }

        // Save workhorse data - directly store workspace GUID (no index remapping needed)
        _workhorseData.Clear();
        foreach (var workhorse in workhorses)
        {
            var animator = WorkhorseAnimator.GetAnimator(workhorse.EntityId);
            int characterIndex = animator?.CharacterIndex ?? -1;

            Debug.Log($"[Save] Workhorse {workhorse.EntityId} ({workhorse.Type}) assigned to workspace {workhorse.AssignedWorkspaceId}, characterIndex {characterIndex}");

            _workhorseData.Add(new SavedWorkhorseData
            {
                EntityId = workhorse.EntityId,
                Type = workhorse.Type,
                AssignedWorkspaceId = workhorse.AssignedWorkspaceId,
                Position = workhorse.Position,
                IsRevealed = workhorse.IsRevealed,
                CharacterIndex = characterIndex
            });
        }

        Debug.Log($"[Save] Saved {_workhorseData.Count} workhorses: {string.Join(", ", _workhorseData.Select(w => w.EntityId))}");
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
