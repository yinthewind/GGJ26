using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class DragDropInputSystem : MonoBehaviour
{
    private enum DragType { None, Skeleton }

    private DragType _dragType = DragType.None;
    private Guid? _draggedEntityId;
    private Camera _mainCamera;
    private WorkspacePreviewAnimator _placementPreview;
    private bool _dragEnabled = true;
    private bool _isPlacingNewWorkspace = false;

    public event Action OnWorkspacePlaced;

    private void Start()
    {
        _mainCamera = Camera.main;
        _placementPreview = WorkspacePreviewAnimator.Create();

        if (ModalManager.Instance != null)
        {
            ModalManager.Instance.OnModalStateChanged += OnModalStateChanged;
        }
    }

    private void Update()
    {
        var mouse = Mouse.current;
        if (mouse == null) return;
        if (!_dragEnabled) return;

        // Handle new workspace placement mode
        if (_isPlacingNewWorkspace)
        {
            UpdateWorkspacePlacement();

            if (mouse.leftButton.wasPressedThisFrame && !IsPointerOverUI())
                TryConfirmWorkspacePlacement();
            else if (Keyboard.current.escapeKey.wasPressedThisFrame ||
                     (mouse.rightButton.wasPressedThisFrame && !IsPointerOverUI()))
                CancelWorkspacePlacement();

            return; // Don't process other input while placing
        }

        if (mouse.leftButton.wasPressedThisFrame)
        {
            // Skip if clicking on UI elements, unless there's a draggable entity
            if (IsPointerOverUI() && !HasDraggableEntityAtMouse())
                return;
            TryStartDrag();
        }
        else if (mouse.leftButton.wasReleasedThisFrame && _draggedEntityId.HasValue)
            EndDrag();
        else if (mouse.leftButton.isPressed && _draggedEntityId.HasValue)
            UpdateDrag();
    }

    private void TryStartDrag()
    {
        if (!_dragEnabled) return;

        var clickWorldPos = GetMouseWorldPosition();

        // Check mode: clicking reveals masked workhorses
        if (CheckModeManager.Instance.IsCheckModeActive)
        {
            if (TryRevealWorkhorse(clickWorldPos))
                return;  // Consumed click, don't start drag
        }

        // Priority 1: Try to start dragging a skeleton
        var controller = GetWorkhorseAtPosition(clickWorldPos);
        if (controller != null)
        {
            _dragType = DragType.Skeleton;
            _draggedEntityId = controller.EntityId;

            // Unassign from workspace when starting to drag
            if (controller.AssignedWorkspaceId.HasValue)
            {
                var assignedWorkspace = WorkspaceControllers.Instance.GetByEntityId(controller.AssignedWorkspaceId.Value);
                assignedWorkspace?.UnassignSkeleton();
                controller.UnassignFromWorkspace();
            }

            controller.SetDragging(true);
            controller.Animator.SetDragIndicator(true);
        }
    }

    private void UpdateDrag()
    {
        var mouseWorldPos = GetMouseWorldPosition();

        if (_dragType == DragType.Skeleton)
        {
            UpdateSkeletonDrag(mouseWorldPos);
        }
    }

    private void UpdateSkeletonDrag(Vector2 mouseWorldPos)
    {
        var controller = CharacterControllers.Instance.GetByEntityId(_draggedEntityId.Value);
        if (controller == null) return;

        controller.SetPosition(new Vector3(mouseWorldPos.x, mouseWorldPos.y, controller.Position.z));

        // Update fire zone highlighting
        var fireZone = WorkhorseShopPanel.Instance?.FireZone;
        if (fireZone != null)
        {
            bool isOverFireZone = fireZone.ContainsScreenPoint(Mouse.current.position.ReadValue());
            fireZone.SetHighlighted(isOverFireZone, isOverFireZone ? controller.Type : null);
        }
    }

    private void EndDrag()
    {
        if (_dragType == DragType.Skeleton)
        {
            EndSkeletonDrag();
        }

        _dragType = DragType.None;
        _draggedEntityId = null;
    }

    private void EndSkeletonDrag()
    {
        var controller = CharacterControllers.Instance.GetByEntityId(_draggedEntityId.Value);
        if (controller == null) return;

        controller.SetDragging(false);
        controller.Animator.SetDragIndicator(false);

        // Check if dropped on fire zone first
        var fireZone = WorkhorseShopPanel.Instance?.FireZone;
        if (fireZone != null)
        {
            if (fireZone.ContainsScreenPoint(Mouse.current.position.ReadValue()))
            {
                fireZone.HandleDrop(controller);
                return;
            }
            // Clear highlighting when drag ends
            fireZone.SetHighlighted(false);
        }

        var dropPos = GetMouseWorldPosition();
        var workspace = WorkspaceControllers.Instance.FindAtPosition(dropPos);

        if (workspace != null)
        {
            // Unassign existing skeleton if workspace is occupied
            if (workspace.IsOccupied)
            {
                var existingSkeleton = CharacterControllers.Instance.GetByEntityId(workspace.AssignedSkeletonId.Value);
                if (existingSkeleton != null)
                {
                    existingSkeleton.UnassignFromWorkspace();
                }
                workspace.UnassignSkeleton();
            }

            // Assign the dropped skeleton to the workspace (centered on workspace)
            workspace.AssignSkeleton(controller.EntityId);
            controller.AssignToWorkspace(workspace.EntityId, workspace.WorldCenter);
        }
    }

    public void EnterWorkspacePlacementMode()
    {
        _isPlacingNewWorkspace = true;
        _placementPreview.Show();
    }

    private void UpdateWorkspacePlacement()
    {
        var mouseWorldPos = GetMouseWorldPosition();
        var gridPosResult = GridSystem.RaycastToGridPosition(new Vector3(mouseWorldPos.x, mouseWorldPos.y, 0f));

        if (gridPosResult.HasValue)
        {
            var gridPos = gridPosResult.Value;
            var snappedPos = GridSystem.GridToWorld(gridPos);
            var isValid = WorkspaceControllers.Instance.IsValidPlacement(gridPos, new Vector2Int(1, 1));
            _placementPreview.UpdatePreview(snappedPos, isValid, gridPos);
        }
        else
        {
            // No floor tile under cursor - show invalid preview at mouse position
            _placementPreview.UpdatePreview(new Vector3(mouseWorldPos.x, mouseWorldPos.y, 0f), false, null);
        }
    }

    private void TryConfirmWorkspacePlacement()
    {
        var gridPos = _placementPreview.GridPosition;
        var gridSize = new Vector2Int(1, 1);

        if (!WorkspaceControllers.Instance.IsValidPlacement(gridPos, gridSize))
            return; // Invalid position, do nothing

        if (!PlayerProgress.Instance.TrySpendDollar(GameSettings.WorkspacePrice))
            return; // Can't afford

        // Spawn workspace at valid position
        var worldPos = GridSystem.GridToWorld(gridPos);
        WorkspaceControllers.Instance.SpawnWorkspace(
            worldPos, gridSize, gridPos, WorkspaceType.Basic,
            $"Workspace{WorkspaceControllers.Instance.Workspaces.Count + 1}");

        // Exit placement mode and notify shop
        _isPlacingNewWorkspace = false;
        _placementPreview.Hide();
        OnWorkspacePlaced?.Invoke();
    }

    private void CancelWorkspacePlacement()
    {
        _isPlacingNewWorkspace = false;
        _placementPreview.Hide();
    }

    private Vector2 GetMouseWorldPosition()
    {
        var mousePos = (Vector3)Mouse.current.position.ReadValue();
        mousePos.z = -_mainCamera.transform.position.z;
        return _mainCamera.ScreenToWorldPoint(mousePos);
    }

    private bool IsPointerOverUI()
    {
        return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(-1);
    }

    private bool HasDraggableEntityAtMouse()
    {
        var clickWorldPos = GetMouseWorldPosition();
        return GetWorkhorseAtPosition(clickWorldPos) != null;
    }

    private WorkhorseController GetWorkhorseAtPosition(Vector2 worldPos)
    {
        var hit = Physics2D.OverlapPoint(worldPos);
        if (hit == null) return null;

        var animator = hit.GetComponentInParent<WorkhorseAnimator>();
        if (animator == null) return null;

        foreach (var controller in CharacterControllers.Instance.Skeletons)
        {
            if (controller.Animator == animator)
                return controller;
        }
        return null;
    }

    private bool TryRevealWorkhorse(Vector2 clickWorldPos)
    {
        var controller = GetWorkhorseAtPosition(clickWorldPos);
        if (controller == null) return false;

        if (controller.IsRevealed)
            return false;

        if (!PlayerProgress.Instance.CanAfford(GameSettings.RevealCost))
        {
            controller.Animator.ShowFloatingText("Need $!");
            return true;
        }

        if (PlayerProgress.Instance.TrySpendDollar(GameSettings.RevealCost))
        {
            controller.Reveal();
            controller.Animator.ShowFloatingText($"-${GameSettings.RevealCost}");
            return true;
        }

        return false;
    }

    private void OnModalStateChanged(bool hasModal)
    {
        _dragEnabled = !hasModal;
    }

    private void OnDestroy()
    {
        if (ModalManager.Instance != null)
        {
            ModalManager.Instance.OnModalStateChanged -= OnModalStateChanged;
        }
    }
}
