using UnityEngine;
using System.Collections.Generic;
using TMPro;
using DG.Tweening;

public class SkeletonAnimator : MonoBehaviour
{
    public static string Tag = "SkeletonAnimator";

    // Static registry
    private static readonly Dictionary<int, SkeletonAnimator> _animators = new();

    public static void Register(int entityId, SkeletonAnimator animator) => _animators[entityId] = animator;
    public static void Unregister(int entityId) => _animators.Remove(entityId);
    public static SkeletonAnimator GetAnimator(int entityId) => _animators.TryGetValue(entityId, out var animator) ? animator : null;
    public static void ClearRegistry() => _animators.Clear();

    // Default skeleton prefab path in Resources
    private const string DefaultPrefabPath = "Addons/BasicPack/2_Prefab/Skelton/SPUM_20240911215639833";

    private SPUM_Prefabs _spumPrefabs;
    private readonly Dictionary<PlayerState, int> _animationIndices = new();
    private SpriteRenderer[] _spriteRenderers;
    private LineRenderer _dragCircle;

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

    public static GameObject Create(Vector3 position, Quaternion rotation, string prefabPath = null, string id = "Unknown")
    {
        var go = new GameObject($"{Tag}:{id}");
        go.transform.SetPositionAndRotation(position, rotation);

        var animator = go.AddComponent<SkeletonAnimator>();
        animator.LoadPrefab(prefabPath ?? DefaultPrefabPath);

        return go;
    }

    private void LoadPrefab(string prefabPath)
    {
        var prefab = Resources.Load<SPUM_Prefabs>(prefabPath);
        if (prefab == null)
        {
            Debug.LogError($"Failed to load SPUM prefab at: {prefabPath}");
            return;
        }

        var instance = Instantiate(prefab, transform);
        instance.transform.localPosition = Vector3.zero;
        instance.transform.localScale = Vector3.one;
        _spumPrefabs = instance;

        InitializeAfterLoad();
    }

    private void InitializeAfterLoad()
    {
        // Initialize animation system
        if (!_spumPrefabs.allListsHaveItemsExist())
        {
            _spumPrefabs.PopulateAnimationLists();
        }
        _spumPrefabs.OverrideControllerInit();

        // Initialize animation indices
        foreach (PlayerState state in System.Enum.GetValues(typeof(PlayerState)))
        {
            _animationIndices[state] = 0;
        }

        // Disable shadow (causes tinting artifacts)
        var shadow = _spumPrefabs.transform.Find("UnitRoot/Shadow");
        if (shadow != null)
        {
            shadow.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogWarning($"SkeletonAnimator {_spumPrefabs.name}: Shadow object not found for disabling.");
        }

        // Cache sprite renderers for tinting
        _spriteRenderers = GetComponentsInChildren<SpriteRenderer>();

        CreateDragCircle();
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

    // Facade API
    public void PlayIdle(int index = 0) => PlayAnimation(PlayerState.IDLE, index);
    public void PlayMove(int index = 0) => PlayAnimation(PlayerState.MOVE, index);
    public void PlayAttack(int index = 0) => PlayAnimation(PlayerState.ATTACK, index);
    public void PlayDamaged(int index = 0) => PlayAnimation(PlayerState.DAMAGED, index);
    public void PlayDebuff(int index = 0) => PlayAnimation(PlayerState.DEBUFF, index);
    public void PlayDeath(int index = 0) => PlayAnimation(PlayerState.DEATH, index);
    public void PlayOther(int index = 0) => PlayAnimation(PlayerState.OTHER, index);

    public void PlayAnimation(PlayerState state, int index = 0)
    {
        if (_spumPrefabs == null) return;
        _animationIndices[state] = index;
        _spumPrefabs.PlayAnimation(state, index);
    }

    public void SetFacing(bool faceRight)
    {
        if (_spumPrefabs == null) return;
        _spumPrefabs.transform.localScale = new Vector3(faceRight ? -1 : 1, 1, 1);
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
}
