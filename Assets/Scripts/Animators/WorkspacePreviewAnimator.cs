using UnityEngine;

public class WorkspacePreviewAnimator : MonoBehaviour
{
    private static readonly Color ValidColor = new Color(0f, 1f, 0f, 0.4f);
    private static readonly Color InvalidColor = new Color(1f, 0f, 0f, 0.4f);

    private Vector2Int _gridSize = Vector2Int.one;
    private SpriteRenderer _spriteRenderer;
    private Transform _visualTransform;

    public Vector2Int GridPosition => GridSystem.WorldToGrid(transform.position);

    public static WorkspacePreviewAnimator Create()
    {
        var go = new GameObject("WorkspacePreviewAnimator");
        var preview = go.AddComponent<WorkspacePreviewAnimator>();
        preview.Initialize();
        return preview;
    }

    private void Initialize()
    {
        var visualGo = new GameObject("Visual");
        visualGo.transform.SetParent(transform);
        _visualTransform = visualGo.transform;
        UpdateVisualTransform();

        _spriteRenderer = visualGo.AddComponent<SpriteRenderer>();
        _spriteRenderer.sprite = CreateWhiteSquareSprite();
        _spriteRenderer.sortingOrder = 10;

        Hide();
    }

    public void SetSize(Vector2Int gridSize)
    {
        _gridSize = gridSize;
        UpdateVisualTransform();
    }

    private void UpdateVisualTransform()
    {
        if (_visualTransform == null) return;

        // Offset visual to center over the full footprint (anchor is bottom-left)
        var offsetX = (_gridSize.x - 1) * 0.5f;
        var offsetY = (_gridSize.y - 1) * 0.5f;
        _visualTransform.localPosition = new Vector3(offsetX, offsetY, 0f);
        _visualTransform.localRotation = Quaternion.Euler(0, 0, 45f);
        _visualTransform.localScale = new Vector3(_gridSize.x, _gridSize.y, 1f);
    }

    private static Sprite CreateWhiteSquareSprite()
    {
        var texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, Color.white);
        texture.Apply();

        return Sprite.Create(texture, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1f);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void UpdatePreview(Vector3 position, bool isValid)
    {
        transform.position = position;
        _spriteRenderer.color = isValid ? ValidColor : InvalidColor;
    }
}
