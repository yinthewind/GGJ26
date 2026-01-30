using UnityEngine;

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
    private static int _nextEntityId = 0;

    private readonly int _entityId;
    private readonly Transform _transform;
    private readonly SkeletonType _type;
    private readonly SkeletonAnimator _animator;

    private SkeletonState _state = SkeletonState.Idle;
    private int? _assignedWorkspaceId;
    private SkeletonState _stateBeforeDrag;
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

    public WorkhorseController(Transform transform, SkeletonType type, SkeletonAnimator animator)
    {
        _entityId = _nextEntityId++;
        _transform = transform;
        _type = type;
        _animator = animator;

        EnterState(SkeletonState.Idle);
    }

    public int EntityId => _entityId;
    public SkeletonType Type => _type;
    public SkeletonAnimator Animator => _animator;
    public SkeletonState State => _state;
    public Vector3 Position => _transform.position;
    public int? AssignedWorkspaceId => _assignedWorkspaceId;

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
            if (Random.value < _attackChance)
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

    private void UpdateFalling(float deltaTime)
    {
        _transform.position += Vector3.down * _fallSpeed * deltaTime;

        if (_transform.position.y <= 0)
        {
            _transform.position = new Vector3(_transform.position.x, 0, _transform.position.z);
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
                _stateTimer = Random.Range(_minIdleTime, _maxIdleTime);
                _animator.PlayIdle();
                break;

            case SkeletonState.Walking:
                _stateTimer = Random.Range(_minWalkTime, _maxWalkTime);
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
        return Random.value > 0.5f ? Vector3.right : Vector3.left;
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
            if (_transform.position.y > 0)
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

    public void AlignToWorkspaceCenter(Vector3 workspaceCenter)
    {
        // Align skeleton bottom with workspace bottom
        // Workspace has center pivot, so its bottom is at y - 0.5f
        // Skeleton has bottom-center pivot (feet), so its bottom is at y
        var alignedPosition = new Vector3(
            workspaceCenter.x,
            workspaceCenter.y - 0.5f,
            workspaceCenter.z
        );
        _transform.position = alignedPosition;
    }

    public void AssignToWorkspace(int workspaceId, Vector3 workspacePosition)
    {
        _assignedWorkspaceId = workspaceId;
        AlignToWorkspaceCenter(workspacePosition);
        EnterState(SkeletonState.Working);
    }

    public void UnassignFromWorkspace()
    {
        _assignedWorkspaceId = null;

        // Fall to ground if above ground level
        if (_transform.position.y > 0)
        {
            EnterState(SkeletonState.Falling);
        }
        else
        {
            EnterState(SkeletonState.Idle);
        }
    }
}
