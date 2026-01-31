using System;

public class CheckModeManager
{
    public static CheckModeManager Instance { get; } = new();

    public bool IsCheckModeActive { get; private set; }

    public event Action<bool> OnCheckModeChanged;
    public event Action<Guid, WorkhorseType> OnWorkhorseRevealed;

    private CheckModeManager() { }

    public void EnterCheckMode()
    {
        if (IsCheckModeActive) return;
        IsCheckModeActive = true;
        OnCheckModeChanged?.Invoke(true);
    }

    public void ExitCheckMode()
    {
        if (!IsCheckModeActive) return;
        IsCheckModeActive = false;
        OnCheckModeChanged?.Invoke(false);
    }

    public void ToggleCheckMode()
    {
        if (IsCheckModeActive)
            ExitCheckMode();
        else
            EnterCheckMode();
    }

    public void NotifyWorkhorseRevealed(Guid entityId, WorkhorseType type)
    {
        OnWorkhorseRevealed?.Invoke(entityId, type);
        ExitCheckMode();
    }
}
