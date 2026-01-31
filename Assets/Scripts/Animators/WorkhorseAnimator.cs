using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class WorkhorseAnimator : MonoBehaviour
{
    public static string Tag = "WorkhorseAnimator";

    // Static registry
    private static readonly Dictionary<Guid, WorkhorseAnimator> _animators = new();

    public static void Register(Guid entityId, WorkhorseAnimator animator) => _animators[entityId] = animator;
    public static void Unregister(Guid entityId) => _animators.Remove(entityId);
    public static WorkhorseAnimator GetAnimator(Guid entityId) => _animators.TryGetValue(entityId, out var animator) ? animator : null;
    public static void ClearRegistry() => _animators.Clear();

    private SpriteRenderer _visualRenderer;
    private SpriteRenderer[] _spriteRenderers;
    private LineRenderer _dragCircle;
    private MaskAnimator _maskAnimator;
    private TextMeshPro _typeLabel;

    private static readonly Color DragCircleColor = new Color(0.3f, 0.7f, 1f, 0.8f);
    private const float CircleRadius = 0.6f;
    private const float CircleLineWidth = 0.05f;
    private const int CircleSegments = 32;
    private const float CircleYOffset = 0.4f;

    // Floating text configuration
    private static readonly Color FloatingTextColor = new Color(0.5f, 0.8f, 1f, 1f); // Light blue
    private const float FloatingTextDuration = 1.0f;
    private const float FloatingTextRiseDistance = 1.0f;
    private const float FloatingTextStartY = 1.0f; // Above skeleton head
    private const float FloatingTextFontSize = 3.6f;

    // Visual configuration
    private const float VisualSize = 1.0f;
    private const float VisualYOffset = 0.5f;
    private const float TypeLabelFontSize = 3f;

    public static GameObject Create(Vector3 position, Quaternion rotation, WorkhorseType type, string id = "Unknown")
    {
        var go = new GameObject($"{Tag}:{id}");
        go.transform.SetPositionAndRotation(position, rotation);

        var animator = go.AddComponent<WorkhorseAnimator>();
        animator.BuildSimpleVisual(type);

        return go;
    }

    private void BuildSimpleVisual(WorkhorseType type)
    {
        // Create Visual child GameObject
        var visualGo = new GameObject("Visual");
        visualGo.transform.SetParent(transform);
        visualGo.transform.localPosition = new Vector3(0f, VisualYOffset, 0f);
        visualGo.transform.localScale = new Vector3(VisualSize, VisualSize, 1f);

        // Add SpriteRenderer with programmatic 1x1 white texture, colored black
        _visualRenderer = visualGo.AddComponent<SpriteRenderer>();
        _visualRenderer.sprite = CreateWhiteSquareSprite();
        _visualRenderer.color = Color.black;
        _visualRenderer.sortingOrder = 1;

        // Create TextMeshPro child for type name
        var labelGo = new GameObject("TypeLabel");
        labelGo.transform.SetParent(visualGo.transform);
        labelGo.transform.localPosition = new Vector3(0f, 0f, -0.1f); // Slightly in front

        _typeLabel = labelGo.AddComponent<TextMeshPro>();
        _typeLabel.text = GetTypeAbbreviation(type);
        _typeLabel.fontSize = TypeLabelFontSize;
        _typeLabel.color = Color.white;
        _typeLabel.alignment = TextAlignmentOptions.Center;
        _typeLabel.sortingOrder = 2;

        // Center the text in the square
        var rectTransform = _typeLabel.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(2f, 1f);

        // Cache sprite renderers for SetVisible/SetTransparent
        _spriteRenderers = new SpriteRenderer[] { _visualRenderer };

        // Add collider for click detection
        var collider = visualGo.AddComponent<BoxCollider2D>();
        collider.size = Vector2.one;

        CreateDragCircle();

        // Create mask for reveal system
        _maskAnimator = MaskAnimator.Create(transform);
    }

    private static Sprite _cachedWhiteSquare;

    public static string GetTypeAbbreviation(WorkhorseType type)
    {
        return type switch
        {
            WorkhorseType.InternNiuma => "IN",
            WorkhorseType.RegularNiuma => "RN",
            WorkhorseType.SuperNiuma => "SN",
            WorkhorseType.ToxicWolf => "TW",
            WorkhorseType.Encourager => "EN",
            WorkhorseType.RisingStar => "RS",
            WorkhorseType.FreeSpirit => "FS",
            WorkhorseType.Pessimist => "PE",
            WorkhorseType.Saboteur => "SA",
            _ => "??"
        };
    }

    private static Sprite CreateWhiteSquareSprite()
    {
        if (_cachedWhiteSquare != null)
            return _cachedWhiteSquare;

        var texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, Color.white);
        texture.Apply();

        _cachedWhiteSquare = Sprite.Create(texture, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1f);
        return _cachedWhiteSquare;
    }

    private void CreateDragCircle()
    {
        var circleGo = new GameObject("DragCircle");
        circleGo.transform.SetParent(transform);
        circleGo.transform.localPosition = new Vector3(0f, CircleYOffset, 0f);

        _dragCircle = circleGo.AddComponent<LineRenderer>();
        _dragCircle.useWorldSpace = false;
        _dragCircle.loop = true;
        _dragCircle.positionCount = CircleSegments;

        for (int i = 0; i < CircleSegments; i++)
        {
            float angle = (float)i / CircleSegments * 2f * Mathf.PI;
            float x = Mathf.Cos(angle) * CircleRadius;
            float y = Mathf.Sin(angle) * CircleRadius;
            _dragCircle.SetPosition(i, new Vector3(x, y, 0f));
        }

        _dragCircle.startWidth = CircleLineWidth;
        _dragCircle.endWidth = CircleLineWidth;
        _dragCircle.startColor = DragCircleColor;
        _dragCircle.endColor = DragCircleColor;
        _dragCircle.material = new Material(Shader.Find("Sprites/Default"));
        _dragCircle.sortingOrder = 5;
        _dragCircle.enabled = false;
    }

    // Facade API - now no-ops since we have no skeletal animation
    public void PlayIdle(int index = 0) { }
    public void PlayMove(int index = 0) { }
    public void PlayAttack(int index = 0) { }
    public void PlayDamaged(int index = 0) { }
    public void PlayDebuff(int index = 0) { }
    public void PlayDeath(int index = 0) { }
    public void PlayOther(int index = 0) { }

    public void PlayAnimation(PlayerState state, int index = 0) { }

    public void SetFacing(bool faceRight) { }

    public void SetDragIndicator(bool visible)
    {
        if (_dragCircle != null)
        {
            _dragCircle.enabled = visible;
        }
    }

    public void SetVisible(bool visible)
    {
        if (_spriteRenderers == null) return;
        foreach (var renderer in _spriteRenderers)
        {
            renderer.enabled = visible;
        }
    }

    public void SetTransparent(bool transparent)
    {
        if (_spriteRenderers == null) return;
        float alpha = transparent ? 0.3f : 1f;
        foreach (var renderer in _spriteRenderers)
        {
            var color = renderer.color;
            color.a = alpha;
            renderer.color = color;
        }
    }

    public void ShowFloatingText(string text)
    {
        // Create TextMeshPro GameObject (world space)
        var textGo = new GameObject("FloatingText");
        textGo.transform.position = transform.position + Vector3.up * FloatingTextStartY;

        var tmp = textGo.AddComponent<TextMeshPro>();
        tmp.text = text;
        tmp.fontSize = FloatingTextFontSize;
        tmp.color = FloatingTextColor;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.sortingOrder = 10;

        // Animate: float up and fade out
        Vector3 targetPos = textGo.transform.position + Vector3.up * FloatingTextRiseDistance;

        Sequence seq = DOTween.Sequence();
        seq.Join(textGo.transform.DOMove(targetPos, FloatingTextDuration).SetEase(Ease.OutQuad));
        seq.Join(tmp.DOFade(0f, FloatingTextDuration).SetEase(Ease.InQuad));
        seq.OnComplete(() => Destroy(textGo));
    }

    public bool IsMaskVisible => _maskAnimator?.IsVisible ?? false;

    public bool ContainsWorldPoint(Vector2 worldPoint)
    {
        if (_spriteRenderers == null) return false;
        foreach (var renderer in _spriteRenderers)
        {
            if (renderer.bounds.Contains(worldPoint))
                return true;
        }
        return false;
    }

    public void SetMaskVisible(bool visible)
    {
        _maskAnimator?.SetVisible(visible);
        SetVisible(!visible);  // Hide real sprites when masked

        // Hide type label when masked (unrevealed)
        if (_typeLabel != null)
        {
            _typeLabel.enabled = !visible;
        }
    }
}
