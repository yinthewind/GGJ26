using System;
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
    public Guid EntityId;
    public AnimatorEventType Type;
    public Vector3 FacingDirection;
}
