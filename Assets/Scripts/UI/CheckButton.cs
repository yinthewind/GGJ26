using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CheckButton : MonoBehaviour
{
    private Button _button;
    private TextMeshProUGUI _text;
    private Image _background;

    // Colors
    private static readonly Color NormalColor = new Color(0.5f, 0.35f, 0.2f, 1f);    // Brown
    private static readonly Color ActiveColor = new Color(0.9f, 0.5f, 0.2f, 1f);     // Orange

    public static CheckButton Create(Transform parent, float width, float height)
    {
        GameObject obj = new GameObject("CheckButton");
        obj.transform.SetParent(parent, false);

        CheckButton component = obj.AddComponent<CheckButton>();
        component.BuildUI(obj, width, height);

        return component;
    }

    private void BuildUI(GameObject root, float width, float height)
    {
        // RectTransform for sizing (positioning handled by parent container)
        RectTransform rect = root.AddComponent<RectTransform>();

        _background = root.AddComponent<Image>();
        _background.color = NormalColor;

        _button = root.AddComponent<Button>();
        _button.targetGraphic = _background;
        _button.onClick.AddListener(HandleClick);

        UpdateButtonColors(false);

        CreateButtonText(root.transform, width, height);

        // Subscribe to check mode changes
        CheckModeManager.Instance.OnCheckModeChanged += HandleCheckModeChanged;
    }

    private void CreateButtonText(Transform parent, float width, float height)
    {
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(parent, false);

        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        _text = textObj.AddComponent<TextMeshProUGUI>();
        _text.text = "CHECK";
        _text.fontSize = 20;
        _text.fontStyle = FontStyles.Bold;
        _text.color = Color.white;
        _text.alignment = TextAlignmentOptions.Center;
    }

    private void HandleClick()
    {
        CheckModeManager.Instance.ToggleCheckMode();
    }

    private void HandleCheckModeChanged(bool isActive)
    {
        UpdateVisual(isActive);
    }

    private void UpdateVisual(bool isActive)
    {
        _text.text = isActive ? "EXIT CHECK" : "CHECK";
        UpdateButtonColors(isActive);
    }

    private void UpdateButtonColors(bool isActive)
    {
        Color baseColor = isActive ? ActiveColor : NormalColor;
        _background.color = baseColor;

        ColorBlock colors = _button.colors;
        colors.normalColor = baseColor;
        colors.highlightedColor = new Color(
            Mathf.Min(baseColor.r + 0.1f, 1f),
            Mathf.Min(baseColor.g + 0.1f, 1f),
            Mathf.Min(baseColor.b + 0.1f, 1f),
            1f);
        colors.pressedColor = new Color(
            baseColor.r * 0.8f,
            baseColor.g * 0.8f,
            baseColor.b * 0.8f,
            1f);
        colors.disabledColor = new Color(0.3f, 0.3f, 0.3f, 0.5f);
        _button.colors = colors;
    }

    private void OnDestroy()
    {
        CheckModeManager.Instance.OnCheckModeChanged -= HandleCheckModeChanged;
    }
}
