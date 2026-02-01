using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProductivityPanel : MonoBehaviour
{
    private TextMeshProUGUI _totalText;
    private Image _background;

    public static ProductivityPanel Create(Transform parent, float width, float height)
    {
        GameObject obj = new GameObject("ProductivityPanel");
        obj.transform.SetParent(parent, false);

        ProductivityPanel component = obj.AddComponent<ProductivityPanel>();
        component.BuildUI(obj, width, height);

        return component;
    }

    private void Start()
    {
        PlayerProgress.Instance.OnProductivityChanged += HandleProductivityChanged;

        UpdateDisplay();
    }

    private void OnDestroy()
    {
        PlayerProgress.Instance.OnProductivityChanged -= HandleProductivityChanged;
    }

    private void BuildUI(GameObject root, float width, float height)
    {
        RectTransform rect = root.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 1f);
        rect.anchorMax = new Vector2(0.5f, 1f);
        rect.pivot = new Vector2(0.5f, 1f);
        rect.sizeDelta = new Vector2(width, height);
        rect.anchoredPosition = new Vector2(0f, -10f);

        _background = root.AddComponent<Image>();
        _background.sprite = SpriteLoader.Instance.GetSprite("Sprites/UIUX/day");
        _background.color = Color.white;

        CreateTotalText(root.transform, width, height);
    }

    private void CreateTotalText(Transform parent, float width, float height)
    {
        GameObject textObj = new GameObject("TotalText");
        textObj.transform.SetParent(parent, false);

        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;
        textRect.offsetMin = new Vector2(10f, 5f);
        textRect.offsetMax = new Vector2(-10f, -5f);

        _totalText = textObj.AddComponent<TextMeshProUGUI>();
        _totalText.text = "Goal: 0/0";
        _totalText.fontSize = 24;
        _totalText.fontStyle = FontStyles.Bold;
        _totalText.color = Color.white;
        _totalText.alignment = TextAlignmentOptions.Center;
    }

    private void HandleProductivityChanged(float totalProductivity)
    {
        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        float current = PlayerProgress.Instance.TotalProductivity;
        float target = GoalManager.Instance.CurrentGoal?.TargetValue ?? 0f;
        _totalText.text = $"Goal: {current:F0}/{target:F0}";
    }
}
