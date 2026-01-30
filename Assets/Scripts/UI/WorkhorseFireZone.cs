using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WorkhorseFireZone : MonoBehaviour
{
    public event Action<WorkhorseController> OnWorkhorseDropped;

    private RectTransform _rectTransform;
    private Image _background;
    private Image _xIcon;
    private TextMeshProUGUI _labelText;
    private TextMeshProUGUI _sellPriceText;
    private bool _isHighlighted;

    public static WorkhorseFireZone Create(Transform parent, float width, float height)
    {
        GameObject obj = new GameObject("WorkhorseFireZone");
        obj.transform.SetParent(parent, false);

        WorkhorseFireZone component = obj.AddComponent<WorkhorseFireZone>();
        component.BuildUI(obj, width, height);

        return component;
    }

    private void BuildUI(GameObject root, float width, float height)
    {
        _rectTransform = root.AddComponent<RectTransform>();
        _rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        _rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        _rectTransform.sizeDelta = new Vector2(width, height);

        _background = root.AddComponent<Image>();
        _background.color = new Color(0.4f, 0.15f, 0.15f, 0.9f);

        // Create X icon
        CreateXIcon(root.transform, height * 0.5f);

        // Create label text
        CreateLabelText(root.transform);

        // Create sell price text (initially hidden)
        CreateSellPriceText(root.transform);
    }

    private void CreateXIcon(Transform parent, float size)
    {
        GameObject xObj = new GameObject("XIcon");
        xObj.transform.SetParent(parent, false);

        RectTransform xRect = xObj.AddComponent<RectTransform>();
        xRect.anchorMin = new Vector2(0.5f, 0.65f);
        xRect.anchorMax = new Vector2(0.5f, 0.65f);
        xRect.sizeDelta = new Vector2(size, size);

        _xIcon = xObj.AddComponent<Image>();
        _xIcon.color = new Color(0.8f, 0.3f, 0.3f);

        // Create X shape using two rotated rectangles
        GameObject line1 = new GameObject("Line1");
        line1.transform.SetParent(xObj.transform, false);
        RectTransform line1Rect = line1.AddComponent<RectTransform>();
        line1Rect.anchorMin = new Vector2(0.5f, 0.5f);
        line1Rect.anchorMax = new Vector2(0.5f, 0.5f);
        line1Rect.sizeDelta = new Vector2(size * 0.8f, size * 0.15f);
        line1Rect.localRotation = Quaternion.Euler(0, 0, 45);
        Image line1Img = line1.AddComponent<Image>();
        line1Img.color = Color.white;

        GameObject line2 = new GameObject("Line2");
        line2.transform.SetParent(xObj.transform, false);
        RectTransform line2Rect = line2.AddComponent<RectTransform>();
        line2Rect.anchorMin = new Vector2(0.5f, 0.5f);
        line2Rect.anchorMax = new Vector2(0.5f, 0.5f);
        line2Rect.sizeDelta = new Vector2(size * 0.8f, size * 0.15f);
        line2Rect.localRotation = Quaternion.Euler(0, 0, -45);
        Image line2Img = line2.AddComponent<Image>();
        line2Img.color = Color.white;

        // Hide the background of the X container
        _xIcon.color = Color.clear;
    }

    private void CreateLabelText(Transform parent)
    {
        GameObject labelObj = new GameObject("Label");
        labelObj.transform.SetParent(parent, false);

        RectTransform labelRect = labelObj.AddComponent<RectTransform>();
        labelRect.anchorMin = new Vector2(0f, 0f);
        labelRect.anchorMax = new Vector2(1f, 0.35f);
        labelRect.sizeDelta = Vector2.zero;
        labelRect.offsetMin = new Vector2(5f, 5f);
        labelRect.offsetMax = new Vector2(-5f, 0f);

        _labelText = labelObj.AddComponent<TextMeshProUGUI>();
        _labelText.text = "Drop to sell";
        _labelText.fontSize = 12;
        _labelText.color = new Color(0.8f, 0.8f, 0.8f);
        _labelText.alignment = TextAlignmentOptions.Center;
    }

    private void CreateSellPriceText(Transform parent)
    {
        GameObject priceObj = new GameObject("SellPrice");
        priceObj.transform.SetParent(parent, false);

        RectTransform priceRect = priceObj.AddComponent<RectTransform>();
        priceRect.anchorMin = new Vector2(0f, 0.35f);
        priceRect.anchorMax = new Vector2(1f, 0.5f);
        priceRect.sizeDelta = Vector2.zero;
        priceRect.offsetMin = new Vector2(5f, 0f);
        priceRect.offsetMax = new Vector2(-5f, 0f);

        _sellPriceText = priceObj.AddComponent<TextMeshProUGUI>();
        _sellPriceText.text = "";
        _sellPriceText.fontSize = 14;
        _sellPriceText.fontStyle = FontStyles.Bold;
        _sellPriceText.color = new Color(1f, 0.85f, 0.3f);
        _sellPriceText.alignment = TextAlignmentOptions.Center;
    }

    public bool ContainsScreenPoint(Vector2 screenPoint)
    {
        // Pass null for Screen Space Overlay canvas (no camera transformation needed)
        return RectTransformUtility.RectangleContainsScreenPoint(_rectTransform, screenPoint, null);
    }

    public void HandleDrop(WorkhorseController controller)
    {
        if (controller == null)
            return;

        int sellPrice = GameSettings.GetSellPrice(controller.Type);
        PlayerProgress.Instance.AddGold(sellPrice);

        OnWorkhorseDropped?.Invoke(controller);

        CharacterControllers.Instance.DestroySkeleton(controller);

        SetHighlighted(false);
    }

    public void SetHighlighted(bool highlighted, WorkhorseType? previewType = null)
    {
        _isHighlighted = highlighted;

        if (highlighted)
        {
            _background.color = new Color(0.5f, 0.2f, 0.2f, 0.95f);

            if (previewType.HasValue)
            {
                int sellPrice = GameSettings.GetSellPrice(previewType.Value);
                _sellPriceText.text = $"+{sellPrice}g";
            }
        }
        else
        {
            _background.color = new Color(0.4f, 0.15f, 0.15f, 0.9f);
            _sellPriceText.text = "";
        }
    }
}
