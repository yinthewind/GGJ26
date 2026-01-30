using UnityEngine;
using UnityEngine.UI;

public class HUDButtonContainer : MonoBehaviour
{
    public static HUDButtonContainer Create(Transform parent, int buttonCount)
    {
        GameObject obj = new GameObject("ButtonContainer");
        obj.transform.SetParent(parent, false);

        HUDButtonContainer container = obj.AddComponent<HUDButtonContainer>();
        container.BuildUI(buttonCount);
        return container;
    }

    private void BuildUI(int buttonCount)
    {
        // Setup RectTransform anchored to left-center
        RectTransform rect = gameObject.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0, 0.5f);
        rect.anchorMax = new Vector2(0, 0.5f);
        rect.pivot = new Vector2(0, 0.5f);
        rect.anchoredPosition = new Vector2(20, 0);

        // Add VerticalLayoutGroup
        var layout = gameObject.AddComponent<VerticalLayoutGroup>();
        layout.spacing = 10;
        layout.childAlignment = TextAnchor.MiddleLeft;
        layout.childControlWidth = false;
        layout.childControlHeight = false;

        // Add ContentSizeFitter for auto-sizing
        var fitter = gameObject.AddComponent<ContentSizeFitter>();
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        // Create button placeholders
        for (int i = 0; i < buttonCount; i++)
        {
            HUDButtonPlaceholder.Create(transform, 120, 40, $"Button {i + 1}");
        }
    }
}
