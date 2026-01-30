using UnityEngine;
using UnityEngine.InputSystem;

public class DragDropInputSystem : MonoBehaviour
{
    private const float HitRadius = 0.75f;

    private enum DragType { None, Skeleton, Workspace }

    private DragType _dragType = DragType.None;
    private int? _draggedEntityId;
    private Camera _mainCamera;
    private WorkspacePreviewAnimator _placementPreview;
    private bool _dragEnabled = true;

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

        if (mouse.leftButton.wasPressedThisFrame)
            TryStartDrag();
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
        foreach (var controller in CharacterControllers.Instance.Skeletons)
        {
            var distance = Vector2.Distance(clickWorldPos, controller.Position);
            if (distance < HitRadius)
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
                return;
            }
        }

        // Priority 2: Try to start dragging a workspace
        var workspace = WorkspaceControllers.Instance.FindAtPosition(clickWorldPos);
        if (workspace != null)
        {
            _dragType = DragType.Workspace;
            _draggedEntityId = workspace.EntityId;
            workspace.OnDragStart();

            // Make workspace transparent during drag
            workspace.Animator.SetTransparent(true);

            // Also make assigned skeleton transparent
            if (workspace.AssignedSkeletonId.HasValue)
            {
                var skeleton = CharacterControllers.Instance.GetByEntityId(workspace.AssignedSkeletonId.Value);
                skeleton?.Animator.SetTransparent(true);
            }

            // Show and configure preview at current position
            _placementPreview.SetSize(workspace.GridSize);
            _placementPreview.Show();
            _placementPreview.UpdatePreview(workspace.Position, true);
        }
    }

    private void UpdateDrag()
    {
        var mouseWorldPos = GetMouseWorldPosition();

        if (_dragType == DragType.Skeleton)
        {
            UpdateSkeletonDrag(mouseWorldPos);
        }
        else if (_dragType == DragType.Workspace)
        {
            UpdateWorkspaceDrag(mouseWorldPos);
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

    private void UpdateWorkspaceDrag(Vector2 mouseWorldPos)
    {
        var workspace = WorkspaceControllers.Instance.GetByEntityId(_draggedEntityId.Value);
        if (workspace == null) return;

        // Only move the preview, NOT the actual workspace or skeleton
        var snappedPos = GridSystem.SnapToGrid(new Vector3(mouseWorldPos.x, mouseWorldPos.y, 0f));
        var previewGridPos = GridSystem.WorldToGrid(snappedPos);

        // Check validity against preview position (workspace hasn't moved)
        var isValid = WorkspaceControllers.Instance.IsValidPlacement(
            previewGridPos, workspace.GridSize, workspace.EntityId);

        _placementPreview.UpdatePreview(snappedPos, isValid);
    }

    private void EndDrag()
    {
        if (_dragType == DragType.Skeleton)
        {
            EndSkeletonDrag();
        }
        else if (_dragType == DragType.Workspace)
        {
            EndWorkspaceDrag();
        }

        _dragType = DragType.None;
        _draggedEntityId = null;
    }

    private void EndSkeletonDrag()
    {
        var controller = CharacterControllers.Instance.GetByEntityId(_draggedEntityId.Value);
        if (controller == null) return;

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
        else
        {
            // No workspace found - continue with existing falling/idle logic
            controller.SetDragging(false);
        }
    }

    private void EndWorkspaceDrag()
    {
        var workspace = WorkspaceControllers.Instance.GetByEntityId(_draggedEntityId.Value);
        if (workspace == null) return;

        _placementPreview.Hide();

        // Get preview position for final validation
        var previewGridPos = _placementPreview.GridPosition;
        var isValid = WorkspaceControllers.Instance.IsValidPlacement(
            previewGridPos, workspace.GridSize, workspace.EntityId);

        if (isValid)
        {
            // Move workspace to new position
            workspace.SetPosition(GridSystem.GridToWorld(previewGridPos));

            // Move assigned skeleton to new center
            if (workspace.AssignedSkeletonId.HasValue)
            {
                var skeleton = CharacterControllers.Instance.GetByEntityId(workspace.AssignedSkeletonId.Value);
                if (skeleton != null) skeleton.AlignToWorkspaceCenter(workspace.WorldCenter);
            }
        }
        // else: workspace never moved, skeleton already at correct position

        workspace.OnDragEnd();

        // Restore workspace opacity
        workspace.Animator.SetTransparent(false);

        // Restore skeleton opacity
        if (workspace.AssignedSkeletonId.HasValue)
        {
            var skeleton = CharacterControllers.Instance.GetByEntityId(workspace.AssignedSkeletonId.Value);
            skeleton?.Animator.SetTransparent(false);
        }
    }

    private Vector2 GetMouseWorldPosition()
    {
        var mousePos = (Vector3)Mouse.current.position.ReadValue();
        mousePos.z = -_mainCamera.transform.position.z;
        return _mainCamera.ScreenToWorldPoint(mousePos);
    }

    private bool TryRevealWorkhorse(Vector2 clickWorldPos)
    {
        foreach (var controller in CharacterControllers.Instance.Skeletons)
        {
            var distance = Vector2.Distance(clickWorldPos, controller.Position);
            if (distance < HitRadius)
            {
                if (controller.IsRevealed)
                    continue;  // Already revealed

                if (!PlayerProgress.Instance.CanAfford(GameSettings.RevealCost))
                {
                    controller.Animator.ShowFloatingText("Need gold!");
                    return true;
                }

                if (PlayerProgress.Instance.TrySpendGold(GameSettings.RevealCost))
                {
                    controller.Reveal();
                    controller.Animator.ShowFloatingText($"-{GameSettings.RevealCost}g");
                    return true;
                }
            }
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
