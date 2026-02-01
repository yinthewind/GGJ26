using UnityEngine;

public class MaskAnimator : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    private Sprite _standingSprite;
    private Sprite _sittingSprite;

    public bool IsVisible => _spriteRenderer != null && _spriteRenderer.enabled;

    public static MaskAnimator Create(Transform parent)
    {
        GameObject obj = new GameObject("Mask");
        obj.transform.SetParent(parent, false);
        obj.transform.localPosition = new Vector3(0f, 0.5f, 0f);

        MaskAnimator mask = obj.AddComponent<MaskAnimator>();
        mask.BuildVisual(obj);

        return mask;
    }

    private void BuildVisual(GameObject root)
    {
        // Load Mask character sprites
        _standingSprite = SpriteLoader.Instance.GetTexture("Sprites/Characters/Mask");
        _sittingSprite = SpriteLoader.Instance.GetTexture("Sprites/Characters/Mask_Sit");

        _spriteRenderer = root.AddComponent<SpriteRenderer>();
        _spriteRenderer.sprite = _standingSprite;
        _spriteRenderer.color = Color.white;

        // Above workhorse sprites
        _spriteRenderer.sortingOrder = 10;

        // Match workhorse visual scale
        root.transform.localScale = new Vector3(1.0f, 1.0f, 1f);

        // Set default visible
        _spriteRenderer.enabled = true;
    }

    public void SetSitting(bool sitting)
    {
        if (_spriteRenderer != null)
        {
            _spriteRenderer.sprite = sitting ? _sittingSprite : _standingSprite;
        }
    }

    public void SetVisible(bool visible)
    {
        if (_spriteRenderer != null)
        {
            _spriteRenderer.enabled = visible;
        }
    }
}
