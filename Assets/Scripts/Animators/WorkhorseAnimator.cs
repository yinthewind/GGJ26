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

    // Character sprite sets (standing, sitting) for random assignment
    private static readonly (string standing, string sitting)[] CharacterSprites = new[]
    {
        ("Sprites/Characters/NiuMa", "Sprites/Characters/NiuMaSit"),
        ("Sprites/Characters/ToxicWolf", "Sprites/Characters/ToxicWolf_Sit"),
        ("Sprites/Characters/Encourager", "Sprites/Characters/Encourager_Sit"),
        ("Sprites/Characters/RisingStar", "Sprites/Characters/RisingStar_Sit"),
        ("Sprites/Characters/FreeSpirit", "Sprites/Characters/FreeSpirit_Sit"),
        ("Sprites/Characters/Pessimist", "Sprites/Characters/pessimist_sit"),
        ("Sprites/Characters/Saboteur", "Sprites/Characters/Saboteur_Sit"),
    };

    private SpriteRenderer _visualRenderer;
    private Sprite _standingSprite;
    private Sprite _sittingSprite;
    private SpriteRenderer[] _spriteRenderers;
    private LineRenderer _dragCircle;
    private MaskAnimator _maskAnimator;

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
        visualGo.transform.localPosition = Vector3.zero;
        visualGo.transform.localScale = new Vector3(VisualSize, VisualSize, 1f);

        // Randomly select a character sprite set
        int characterIndex = UnityEngine.Random.Range(0, CharacterSprites.Length);
        var (standingPath, sittingPath) = CharacterSprites[characterIndex];
        _standingSprite = SpriteLoader.Instance.GetTexture(standingPath);
        _sittingSprite = SpriteLoader.Instance.GetTexture(sittingPath);

        // Add SpriteRenderer with standing sprite (default state)
        _visualRenderer = visualGo.AddComponent<SpriteRenderer>();
        _visualRenderer.sprite = _standingSprite;
        _visualRenderer.color = Color.white;  // Use sprite's natural colors
        _visualRenderer.sortingOrder = 1;

        // Cache sprite renderers for SetVisible/SetTransparent
        _spriteRenderers = new SpriteRenderer[] { _visualRenderer };

        // Add collider for click detection
        var collider = visualGo.AddComponent<BoxCollider2D>();
        collider.size = Vector2.one;

        CreateDragCircle();

        // Create mask for reveal system
        _maskAnimator = MaskAnimator.Create(transform);
    }

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

    public void SetSitting(bool sitting)
    {
        if (_visualRenderer != null)
        {
            _visualRenderer.sprite = sitting ? _sittingSprite : _standingSprite;
        }
        _maskAnimator?.SetSitting(sitting);
    }

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
    }
}
