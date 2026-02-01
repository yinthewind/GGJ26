using UnityEngine;

public class WorkspacePreviewAnimator : MonoBehaviour
{
    private static readonly Color DeskPreviewColor = new Color(1f, 1f, 1f, 0.5f);

    private Vector2Int? _currentHighlightedPosition;
    private SpriteRenderer _deskPreview;

    public Vector2Int GridPosition => _currentHighlightedPosition ?? Vector2Int.zero;

    public static WorkspacePreviewAnimator Create()
    {
        var go = new GameObject("WorkspacePreviewAnimator");
        var preview = go.AddComponent<WorkspacePreviewAnimator>();
        preview.Initialize();
        preview.Hide();
        return preview;
    }

    private void Initialize()
    {
        _deskPreview = gameObject.AddComponent<SpriteRenderer>();
        _deskPreview.sprite = SpriteLoader.Instance.GetSprite("Sprites/desk");
        _deskPreview.color = DeskPreviewColor;
        _deskPreview.sortingOrder = 10;
        _deskPreview.enabled = false;
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        ClearCurrentHighlight();
        _deskPreview.enabled = false;
        gameObject.SetActive(false);
    }

    public void UpdatePreview(Vector3 position, bool isValid, Vector2Int? gridPosition = null)
    {
        // Clear previous highlight if position changed
        if (_currentHighlightedPosition != gridPosition)
            ClearCurrentHighlight();

        // Set new highlight if we have a valid grid position
        if (gridPosition.HasValue)
        {
            var highlightType = isValid ? HighlightType.Valid : HighlightType.Invalid;
            FloorGridAnimatorManager.SetHighlight(gridPosition.Value, highlightType);
            _currentHighlightedPosition = gridPosition;
        }

        // Position and show desk preview only when valid
        transform.position = position;
        _deskPreview.enabled = isValid && gridPosition.HasValue;
    }

    private void ClearCurrentHighlight()
    {
        if (_currentHighlightedPosition.HasValue)
        {
            FloorGridAnimatorManager.ClearHighlight(_currentHighlightedPosition.Value);
            _currentHighlightedPosition = null;
        }
    }
}
