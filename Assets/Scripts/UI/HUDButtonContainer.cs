using UnityEngine;
using UnityEngine.UI;

public class HUDButtonContainer : MonoBehaviour
{
    private RectTransform _rectTransform;

    public static HUDButtonContainer Create(Transform parent, float width)
    {
        GameObject obj = new GameObject("HUDButtonContainer");
        obj.transform.SetParent(parent, false);

        HUDButtonContainer container = obj.AddComponent<HUDButtonContainer>();
        container.BuildUI(width);
        return container;
    }

    private void BuildUI(float width)
    {
        // Setup RectTransform anchored to left-center
        _rectTransform = gameObject.AddComponent<RectTransform>();
        _rectTransform.anchorMin = new Vector2(0, 0.5f);
        _rectTransform.anchorMax = new Vector2(0, 0.5f);
        _rectTransform.pivot = new Vector2(0, 0.5f);
        _rectTransform.anchoredPosition = new Vector2(20, 0);
        _rectTransform.sizeDelta = new Vector2(width, 0f);

        // Add VerticalLayoutGroup
        var layout = gameObject.AddComponent<VerticalLayoutGroup>();
        layout.spacing = 10;
        layout.childAlignment = TextAnchor.MiddleLeft;
        layout.childControlWidth = true;
        layout.childControlHeight = false;
        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = false;

        // Add ContentSizeFitter for auto-sizing
        var fitter = gameObject.AddComponent<ContentSizeFitter>();
        fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
    }

    public void AddButton(GameObject button, float height)
    {
        button.transform.SetParent(transform, false);

        // Add LayoutElement to control height
        LayoutElement layoutElement = button.AddComponent<LayoutElement>();
        layoutElement.preferredHeight = height;
    }
}
