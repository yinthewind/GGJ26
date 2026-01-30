using System;

/// <summary>
/// Stub for ModalManager - handles modal dialog state.
/// The DragDropInputSystem uses this to disable dragging when modals are open.
/// </summary>
public class ModalManager
{
    public static ModalManager Instance { get; private set; }

    public event Action<bool> OnModalStateChanged;

    public static void Initialize()
    {
        Instance = new ModalManager();
    }

    public void NotifyModalStateChanged(bool hasModal)
    {
        OnModalStateChanged?.Invoke(hasModal);
    }
}
