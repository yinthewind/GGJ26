using UnityEngine;

public enum AnimatorEventType
{
    Idle,
    Move,
    Attack,
    Damaged,
    Death,
    Working,
}

public struct AnimatorEvent
{
    public int EntityId;
    public AnimatorEventType Type;
    public Vector3 FacingDirection;
}
