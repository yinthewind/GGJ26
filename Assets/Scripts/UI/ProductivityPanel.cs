using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProductivityPanel : MonoBehaviour
{
    private TextMeshProUGUI _totalText;
    private TextMeshProUGUI _previewText;
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
        TurnManager.Instance.OnTurnStarted += UpdatePreview;
        TurnManager.Instance.OnTurnEnded += UpdatePreview;

        UpdateDisplay();
        UpdatePreview();
    }

    private void OnDestroy()
    {
        PlayerProgress.Instance.OnProductivityChanged -= HandleProductivityChanged;
        TurnManager.Instance.OnTurnStarted -= UpdatePreview;
        TurnManager.Instance.OnTurnEnded -= UpdatePreview;
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
        _background.color = new Color(0.1f, 0.1f, 0.15f, 0.9f);

        CreateTotalText(root.transform, width, height);
        CreatePreviewText(root.transform, width, height);
    }

    private void CreateTotalText(Transform parent, float width, float height)
    {
        GameObject textObj = new GameObject("TotalText");
        textObj.transform.SetParent(parent, false);

        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = new Vector2(0f, 0.5f);
        textRect.anchorMax = new Vector2(1f, 1f);
        textRect.sizeDelta = Vector2.zero;
        textRect.offsetMin = new Vector2(10f, 0f);
        textRect.offsetMax = new Vector2(-10f, -5f);

        _totalText = textObj.AddComponent<TextMeshProUGUI>();
        _totalText.text = "Total: 0";
        _totalText.fontSize = 20;
        _totalText.fontStyle = FontStyles.Bold;
        _totalText.color = new Color(1f, 0.85f, 0.3f);
        _totalText.alignment = TextAlignmentOptions.Center;
    }

    private void CreatePreviewText(Transform parent, float width, float height)
    {
        GameObject textObj = new GameObject("PreviewText");
        textObj.transform.SetParent(parent, false);

        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = new Vector2(0f, 0f);
        textRect.anchorMax = new Vector2(1f, 0.5f);
        textRect.sizeDelta = Vector2.zero;
        textRect.offsetMin = new Vector2(10f, 5f);
        textRect.offsetMax = new Vector2(-10f, 0f);

        _previewText = textObj.AddComponent<TextMeshProUGUI>();
        _previewText.text = "Next: +0";
        _previewText.fontSize = 16;
        _previewText.color = new Color(0.7f, 0.9f, 0.7f);
        _previewText.alignment = TextAlignmentOptions.Center;
    }

    private void Update()
    {
        UpdatePreview();
    }

    private void HandleProductivityChanged(float totalProductivity)
    {
        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        _totalText.text = $"Total: {PlayerProgress.Instance.TotalProductivity:F1}";
    }

    private void UpdatePreview()
    {
        float preview = TurnManager.Instance.PreviewProductivity();
        _previewText.text = $"Next: +{preview:F1}";

        if (preview > 0)
        {
            _previewText.color = new Color(0.7f, 0.9f, 0.7f);
        }
        else
        {
            _previewText.color = new Color(0.6f, 0.6f, 0.6f);
        }
    }
}
