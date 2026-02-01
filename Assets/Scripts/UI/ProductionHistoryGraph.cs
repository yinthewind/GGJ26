using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProductionHistoryGraph : MonoBehaviour
{
    private float _width;
    private float _height;
    private Color _barColor;
    private float _spacing;
    private RectTransform _containerRect;
    private HorizontalLayoutGroup _layoutGroup;
    private List<GameObject> _bars = new List<GameObject>();

    public static ProductionHistoryGraph Create(
        Transform parent,
        float width,
        float height,
        Color barColor,
        float spacing)
    {
        GameObject obj = new GameObject("ProductionHistoryGraph");
        obj.transform.SetParent(parent, false);

        ProductionHistoryGraph component = obj.AddComponent<ProductionHistoryGraph>();
        component._width = width;
        component._height = height;
        component._barColor = barColor;
        component._spacing = spacing;
        component.BuildUI(obj);

        return component;
    }

    private void BuildUI(GameObject root)
    {
        // Create RectTransform with center anchors + explicit size
        _containerRect = root.AddComponent<RectTransform>();
        _containerRect.anchorMin = new Vector2(0.5f, 0.5f);
        _containerRect.anchorMax = new Vector2(0.5f, 0.5f);
        _containerRect.sizeDelta = new Vector2(_width, _height);

        // Add HorizontalLayoutGroup for automatic bar spacing
        _layoutGroup = root.AddComponent<HorizontalLayoutGroup>();
        _layoutGroup.spacing = _spacing;
        _layoutGroup.childAlignment = TextAnchor.LowerCenter;
        _layoutGroup.childControlWidth = false;
        _layoutGroup.childControlHeight = false;
        _layoutGroup.childForceExpandWidth = false;
        _layoutGroup.childForceExpandHeight = false;
    }

    public void SetData(List<float> values)
    {
        // Clear existing bars
        foreach (var bar in _bars)
        {
            Destroy(bar);
        }
        _bars.Clear();

        if (values == null || values.Count == 0)
        {
            return;
        }

        // Find max value for proportional scaling
        float maxValue = 0f;
        foreach (var value in values)
        {
            if (value > maxValue)
            {
                maxValue = value;
            }
        }

        // Calculate bar width: (totalWidth - (barCount-1)*spacing) / barCount
        int barCount = values.Count;
        float barWidth = (_width - (barCount - 1) * _spacing) / barCount;

        // Create bars
        foreach (var value in values)
        {
            GameObject bar = CreateBar(value, maxValue, barWidth);
            _bars.Add(bar);
        }
    }

    private GameObject CreateBar(float value, float maxValue, float barWidth)
    {
        // Create container for bar + label
        GameObject container = new GameObject("BarContainer");
        container.transform.SetParent(transform, false);

        RectTransform containerRect = container.AddComponent<RectTransform>();
        containerRect.anchorMin = new Vector2(0.5f, 0f);
        containerRect.anchorMax = new Vector2(0.5f, 0f);
        containerRect.pivot = new Vector2(0.5f, 0f);
        containerRect.sizeDelta = new Vector2(barWidth, _height);

        // Add LayoutElement to control size in parent layout group
        LayoutElement containerLayout = container.AddComponent<LayoutElement>();
        containerLayout.preferredWidth = barWidth;
        containerLayout.preferredHeight = _height;

        // Create the bar image
        GameObject barObj = new GameObject("Bar");
        barObj.transform.SetParent(container.transform, false);

        RectTransform barRect = barObj.AddComponent<RectTransform>();
        barRect.anchorMin = new Vector2(0.5f, 0f);
        barRect.anchorMax = new Vector2(0.5f, 0f);
        barRect.pivot = new Vector2(0.5f, 0f);

        float labelHeight = 16f;
        float barHeight = maxValue > 0 ? (value / maxValue) * (_height - labelHeight) : 0f;
        barRect.sizeDelta = new Vector2(barWidth, barHeight);
        barRect.anchoredPosition = new Vector2(0f, labelHeight);

        Image image = barObj.AddComponent<Image>();
        image.color = _barColor;

        // Create the value label below the bar
        GameObject labelObj = new GameObject("ValueLabel");
        labelObj.transform.SetParent(container.transform, false);

        RectTransform labelRect = labelObj.AddComponent<RectTransform>();
        labelRect.anchorMin = new Vector2(0.5f, 0f);
        labelRect.anchorMax = new Vector2(0.5f, 0f);
        labelRect.pivot = new Vector2(0.5f, 0f);
        labelRect.sizeDelta = new Vector2(barWidth, labelHeight);
        labelRect.anchoredPosition = Vector2.zero;

        TextMeshProUGUI label = labelObj.AddComponent<TextMeshProUGUI>();
        label.text = value.ToString("F1");
        label.fontSize = 12f;
        label.alignment = TextAlignmentOptions.Center;
        label.color = Color.white;

        return container;
    }
}
