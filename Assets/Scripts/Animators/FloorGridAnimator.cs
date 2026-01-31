using UnityEngine;

/// <summary>
/// Creates a single floor sprite background that covers all 9 workspace positions
/// in the diamond grid area.
/// </summary>
public class FloorGridAnimator : MonoBehaviour
{
    public static string Tag = "FloorGridAnimator";

    private SpriteRenderer _spriteRenderer;

    /// <summary>
    /// Creates the floor grid background centered at (0, 0).
    /// The 9 valid positions span from (-2, -2) to (2, 2), so the floor
    /// covers a 5x5 grid area, rotated 45 degrees to match the diamond grid aesthetic.
    /// </summary>
    public static GameObject Create()
    {
        var go = new GameObject(Tag);
        go.transform.position = Vector3.zero;

        var animator = go.AddComponent<FloorGridAnimator>();
        animator.CreateVisual();

        return go;
    }

    private void CreateVisual()
    {
        var visualGo = new GameObject("Visual");
        visualGo.transform.SetParent(transform);
        visualGo.transform.localPosition = Vector3.zero;

        // Scale to cover the diamond area (positions span -2 to 2, plus 0.5 margin on each side)
        // The diamond spans 5 units, so we scale to cover that area
        visualGo.transform.localScale = new Vector3(5f, 5f, 1f);

        _spriteRenderer = visualGo.AddComponent<SpriteRenderer>();
        _spriteRenderer.sprite = SpriteLoader.Instance.GetSprite("Sprites/floor");
        _spriteRenderer.color = Color.white;
        _spriteRenderer.sortingOrder = -2; // Behind workspace desks which are -1
    }
}
