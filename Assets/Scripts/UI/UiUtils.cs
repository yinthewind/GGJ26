using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public static class UiUtils
{
    public static Button CreateTextButton(
        GameObject root,
        string text,
        Color color,
        Action callback,
        int fontSize = 20)
    {
        root.AddComponent<RectTransform>();

        Image background = root.AddComponent<Image>();
        background.color = color;

        Button button = root.AddComponent<Button>();
        button.targetGraphic = background;
        button.onClick.AddListener(() => callback());
        button.colors = CreateColorBlock(color);

        CreateButtonText(root.transform, text, fontSize);

        return button;
    }

    public static ColorBlock CreateColorBlock(Color baseColor)
    {
        ColorBlock colors = ColorBlock.defaultColorBlock;
        colors.normalColor = baseColor;
        colors.highlightedColor = new Color(
            Mathf.Min(baseColor.r + 0.1f, 1f),
            Mathf.Min(baseColor.g + 0.1f, 1f),
            Mathf.Min(baseColor.b + 0.1f, 1f), 1f);
        colors.pressedColor = new Color(
            baseColor.r * 0.8f,
            baseColor.g * 0.8f,
            baseColor.b * 0.8f, 1f);
        colors.disabledColor = new Color(0.3f, 0.3f, 0.3f, 0.5f);
        return colors;
    }

    private static void CreateButtonText(Transform parent, string text, int fontSize)
    {
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(parent, false);

        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.fontStyle = FontStyles.Bold;
        tmp.color = Color.white;
        tmp.alignment = TextAlignmentOptions.Center;
    }
}
