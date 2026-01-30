using UnityEngine;
using UnityEngine.UI;

public class ButtonContainer : MonoBehaviour
{
    private RectTransform _rectTransform;
    private VerticalLayoutGroup _layoutGroup;

    public static ButtonContainer Create(Transform parent, float width)
    {
        GameObject obj = new GameObject("ButtonContainer");
        obj.transform.SetParent(parent, false);

        ButtonContainer component = obj.AddComponent<ButtonContainer>();
        component.BuildUI(obj, width);

        return component;
    }

    private void BuildUI(GameObject root, float width)
    {
        _rectTransform = root.AddComponent<RectTransform>();
        // Position: bottom-right corner
        _rectTransform.anchorMin = new Vector2(1f, 0f);
        _rectTransform.anchorMax = new Vector2(1f, 0f);
        _rectTransform.pivot = new Vector2(1f, 0f);
        _rectTransform.anchoredPosition = new Vector2(-20f, 20f);

        // VerticalLayoutGroup for automatic stacking
        _layoutGroup = root.AddComponent<VerticalLayoutGroup>();
        _layoutGroup.childAlignment = TextAnchor.LowerRight;
        _layoutGroup.reverseArrangement = true;  // Bottom-to-top stacking
        _layoutGroup.spacing = 10f;
        _layoutGroup.childControlWidth = true;
        _layoutGroup.childControlHeight = false;
        _layoutGroup.childForceExpandWidth = true;
        _layoutGroup.childForceExpandHeight = false;

        // ContentSizeFitter to auto-size based on children
        ContentSizeFitter fitter = root.AddComponent<ContentSizeFitter>();
        fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        // Set width
        _rectTransform.sizeDelta = new Vector2(width, 0f);
    }

    public void AddButton(GameObject button, float height)
    {
        button.transform.SetParent(transform, false);

        // Add LayoutElement to control height
        LayoutElement layoutElement = button.AddComponent<LayoutElement>();
        layoutElement.preferredHeight = height;
    }
}
