using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDButtonPlaceholder : MonoBehaviour
{
    public static HUDButtonPlaceholder Create(Transform parent, float width, float height, string label)
    {
        GameObject obj = new GameObject("ButtonPlaceholder");
        obj.transform.SetParent(parent, false);

        HUDButtonPlaceholder button = obj.AddComponent<HUDButtonPlaceholder>();
        button.BuildUI(width, height, label);
        return button;
    }

    private void BuildUI(float width, float height, string label)
    {
        RectTransform rect = gameObject.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(width, height);

        // LayoutElement for VerticalLayoutGroup sizing
        var layoutElement = gameObject.AddComponent<LayoutElement>();
        layoutElement.preferredWidth = width;
        layoutElement.preferredHeight = height;

        // Background image
        var image = gameObject.AddComponent<Image>();
        image.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);

        // Button component
        var button = gameObject.AddComponent<Button>();

        // Text label (child object)
        GameObject textObj = new GameObject("Label");
        textObj.transform.SetParent(transform, false);

        var textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;

        var text = textObj.AddComponent<TextMeshProUGUI>();
        text.text = label;
        text.alignment = TextAlignmentOptions.Center;
        text.fontSize = 16;
        text.color = Color.white;
    }
}
