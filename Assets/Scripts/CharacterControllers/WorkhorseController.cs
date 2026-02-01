using System;
using UnityEngine;
using DG.Tweening;

public enum SkeletonState
{
    Idle,
    Walking,
    Attacking,
    Dragging,
    Falling,
    Working,
}

public class WorkhorseController
{
    private readonly Guid _entityId;
    private readonly Transform _transform;
    private readonly WorkhorseType _type;
    private readonly WorkhorseAnimator _animator;

    private SkeletonState _state = SkeletonState.Idle;
    private Guid? _assignedWorkspaceId;
    private SkeletonState _stateBeforeDrag;
    private bool _isRevealed = false;
    private int _roundsWorked = 0;
    private float _stateTimer;
    private Vector3 _walkDirection;
    private float _walkSpeed = 1.5f;
    private float _fallSpeed = 9f;

    // Timing configuration
    private float _minIdleTime = 1f;
    private float _maxIdleTime = 3f;
    private float _minWalkTime = 1f;
    private float _maxWalkTime = 4f;
    private float _attackChance = 0.2f;
    private float _workingAnimationInterval = 0.8f;

    public WorkhorseController(Transform transform, WorkhorseType type, WorkhorseAnimator animator)
        : this(Guid.NewGuid(), transform, type, animator)
    {
    }

    public WorkhorseController(Guid entityId, Transform transform, WorkhorseType type, WorkhorseAnimator animator)
    {
        _entityId = entityId;
        _transform = transform;
        _type = type;
        _animator = animator;

        EnterState(SkeletonState.Idle);

        // Normal NiuMa types are always revealed, others start masked
        if (GameSettings.WorkhorseAlwaysRevealed[_type])
        {
            _isRevealed = true;
            _animator.SetMaskVisible(false);
        }
        else
        {
            _animator.SetMaskVisible(true);
        }
    }

    public Guid EntityId => _entityId;
    public WorkhorseType Type => _type;
    public WorkhorseAnimator Animator => _animator;
    public SkeletonState State => _state;
    public Transform Transform => _transform;
    public Vector3 Position => _transform.position;
    public Guid? AssignedWorkspaceId => _assignedWorkspaceId;
    public bool IsRevealed => _isRevealed;
    public int RoundsWorked => _roundsWorked;

    public void Update(float deltaTime)
    {
        if (_state == SkeletonState.Dragging)
            return;

        _stateTimer -= deltaTime;

        switch (_state)
        {
            case SkeletonState.Idle:
                UpdateIdle();
                break;
            case SkeletonState.Walking:
                UpdateWalking(deltaTime);
                break;
            case SkeletonState.Attacking:
                UpdateAttacking();
                break;
            case SkeletonState.Falling:
                UpdateFalling(deltaTime);
                break;
            case SkeletonState.Working:
                UpdateWorking();
                break;
        }
    }

    private void UpdateIdle()
    {
        if (_stateTimer <= 0)
        {
            if (UnityEngine.Random.value < _attackChance)
            {
                EnterState(SkeletonState.Attacking);
            }
            else
            {
                EnterState(SkeletonState.Walking);
            }
        }
    }

    private void UpdateWalking(float deltaTime)
    {
        _transform.position += _walkDirection * _walkSpeed * deltaTime;

        if (_stateTimer <= 0)
        {
            EnterState(SkeletonState.Idle);
        }
    }

    private void UpdateAttacking()
    {
        if (_stateTimer <= 0)
        {
            EnterState(SkeletonState.Idle);
        }
    }

    private const float GroundY = -3f;

    private void UpdateFalling(float deltaTime)
    {
        _transform.position += Vector3.down * _fallSpeed * deltaTime;

        if (_transform.position.y <= GroundY)
        {
            _transform.position = new Vector3(_transform.position.x, GroundY, _transform.position.z);
            EnterState(SkeletonState.Idle);
        }
    }

    private void UpdateWorking()
    {
        if (_stateTimer <= 0)
        {
            _stateTimer = _workingAnimationInterval;
            _animator.PlayOther(0);
        }
    }

    private void EnterState(SkeletonState newState)
    {
        _state = newState;

        switch (newState)
        {
            case SkeletonState.Idle:
                _stateTimer = UnityEngine.Random.Range(_minIdleTime, _maxIdleTime);
                _animator.PlayIdle();
                break;

            case SkeletonState.Walking:
                _stateTimer = UnityEngine.Random.Range(_minWalkTime, _maxWalkTime);
                _walkDirection = PickRandomDirection();
                _animator.SetFacing(_walkDirection.x > 0);
                _animator.PlayMove();
                break;

            case SkeletonState.Attacking:
                _stateTimer = 1f;
                _animator.PlayAttack();
                break;

            case SkeletonState.Falling:
                _animator.PlayIdle();
                break;

            case SkeletonState.Working:
                _stateTimer = _workingAnimationInterval;
                _animator.PlayOther(0);
                break;
        }
    }

    private Vector3 PickRandomDirection()
    {
        const float edgeMargin = 1f;
        float posX = _transform.position.x;

        // Near left edge - must go right
        if (posX <= CameraBounds.MinX + edgeMargin)
            return Vector3.right;

        // Near right edge - must go left
        if (posX >= CameraBounds.MaxX - edgeMargin)
            return Vector3.left;

        // In the middle - random direction
        return UnityEngine.Random.value > 0.5f ? Vector3.right : Vector3.left;
    }

    public void SetDragging(bool dragging)
    {
        if (dragging)
        {
            _stateBeforeDrag = _state;
            _state = SkeletonState.Dragging;
        }
        else
        {
            if (_transform.position.y > GroundY)
            {
                EnterState(SkeletonState.Falling);
            }
            else
            {
                EnterState(SkeletonState.Idle);
            }
        }
    }

    public void SetPosition(Vector3 position)
    {
        _transform.position = position;
    }

    public void StartFallingIfAboveGround()
    {
        if (_transform.position.y > GroundY)
        {
            EnterState(SkeletonState.Falling);
        }
    }

    public void AlignToWorkspaceCenter(Vector3 workspaceCenter)
    {
        // Apply workhorse pivot offset (convert from pixel space to world units)
        var offset = WorkspacePositionMap.WorkhorsePivotOffset / 100f;
        var alignedPosition = new Vector3(
            workspaceCenter.x + offset.x,
            workspaceCenter.y + offset.y,
            workspaceCenter.z
        );
        _transform.position = alignedPosition;
    }

    public void AssignToWorkspace(Guid workspaceId, Vector3 workspacePosition)
    {
        Debug.Log($"[Assign] Workhorse {_entityId} -> Workspace {workspaceId} at {workspacePosition}");
        DOTween.Kill(_transform);  // Stop any running position animations
        _assignedWorkspaceId = workspaceId;
        AlignToWorkspaceCenter(workspacePosition);
        EnterState(SkeletonState.Working);
    }

    public void UnassignFromWorkspace()
    {
        Debug.Log($"[Unassign] Workhorse {_entityId} unassigned from workspace {_assignedWorkspaceId}");
        _assignedWorkspaceId = null;
        ResetRoundsWorked();

        // Fall to ground if above ground level
        if (_transform.position.y > GroundY)
        {
            EnterState(SkeletonState.Falling);
        }
        else
        {
            EnterState(SkeletonState.Idle);
        }
    }

    public void Reveal()
    {
        if (_isRevealed) return;
        _isRevealed = true;
        _animator.SetMaskVisible(false);
        CheckModeManager.Instance.NotifyWorkhorseRevealed(_entityId, _type);
    }

    public void IncrementRoundsWorked()
    {
        _roundsWorked++;
    }

    public void ResetRoundsWorked()
    {
        _roundsWorked = 0;
    }
}
