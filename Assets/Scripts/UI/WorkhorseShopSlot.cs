using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WorkhorseShopSlot : MonoBehaviour
{
    public event Action<WorkhorseShopSlot> OnBuyClicked;

    private Image _background;
    private Image _iconImage;
    private TextMeshProUGUI _nameText;
    private TextMeshProUGUI _priceText;
    private TextMeshProUGUI _productivityText;
    private Button _buyButton;
    private TextMeshProUGUI _buyButtonText;
    private GameObject _lockedOverlay;
    private TextMeshProUGUI _lockedText;

    private WorkhorseType? _stockedType;
    private bool _isLocked;
    private bool _isEmpty;

    public WorkhorseType? StockedType => _stockedType;
    public bool IsLocked => _isLocked;
    public bool IsEmpty => _isEmpty;

    public static WorkhorseShopSlot Create(Transform parent, float width, float height)
    {
        GameObject obj = new GameObject("WorkhorseShopSlot");
        obj.transform.SetParent(parent, false);

        WorkhorseShopSlot component = obj.AddComponent<WorkhorseShopSlot>();
        component.BuildUI(obj, width, height);

        return component;
    }

    private void BuildUI(GameObject root, float width, float height)
    {
        RectTransform rect = root.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(width, height);

        _background = root.AddComponent<Image>();
        _background.color = new Color(0.15f, 0.15f, 0.2f, 0.95f);

        // Horizontal layout
        HorizontalLayoutGroup layout = root.AddComponent<HorizontalLayoutGroup>();
        layout.padding = new RectOffset(8, 8, 4, 4);
        layout.spacing = 8f;
        layout.childAlignment = TextAnchor.MiddleLeft;
        layout.childControlWidth = false;
        layout.childControlHeight = false;
        layout.childForceExpandWidth = false;
        layout.childForceExpandHeight = false;

        float iconSize = height - 8f;
        CreateIcon(root.transform, iconSize);
        CreateInfoSection(root.transform, width - iconSize - 80f - 32f, height - 8f);
        CreateBuyButton(root.transform, 60f, height - 12f);
        CreateLockedOverlay(root.transform, width, height);
    }

    private void CreateIcon(Transform parent, float size)
    {
        GameObject iconObj = new GameObject("Icon");
        iconObj.transform.SetParent(parent, false);

        RectTransform iconRect = iconObj.AddComponent<RectTransform>();
        iconRect.sizeDelta = new Vector2(size, size);

        _iconImage = iconObj.AddComponent<Image>();
        _iconImage.color = Color.gray;
    }

    private void CreateInfoSection(Transform parent, float width, float height)
    {
        GameObject infoObj = new GameObject("Info");
        infoObj.transform.SetParent(parent, false);

        RectTransform infoRect = infoObj.AddComponent<RectTransform>();
        infoRect.sizeDelta = new Vector2(width, height);

        VerticalLayoutGroup layout = infoObj.AddComponent<VerticalLayoutGroup>();
        layout.childAlignment = TextAnchor.MiddleLeft;
        layout.childControlWidth = true;
        layout.childControlHeight = true;
        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = true;
        layout.spacing = 0f;

        // Name text
        GameObject nameObj = new GameObject("Name");
        nameObj.transform.SetParent(infoObj.transform, false);
        _nameText = nameObj.AddComponent<TextMeshProUGUI>();
        _nameText.text = "Swordsman";
        _nameText.fontSize = 14;
        _nameText.fontStyle = FontStyles.Bold;
        _nameText.color = Color.white;
        _nameText.alignment = TextAlignmentOptions.Left;

        // Stats row
        GameObject statsObj = new GameObject("Stats");
        statsObj.transform.SetParent(infoObj.transform, false);

        HorizontalLayoutGroup statsLayout = statsObj.AddComponent<HorizontalLayoutGroup>();
        statsLayout.childAlignment = TextAnchor.MiddleLeft;
        statsLayout.childControlWidth = false;
        statsLayout.childControlHeight = true;
        statsLayout.childForceExpandWidth = false;
        statsLayout.spacing = 8f;

        // Price text
        GameObject priceObj = new GameObject("Price");
        priceObj.transform.SetParent(statsObj.transform, false);
        RectTransform priceRect = priceObj.AddComponent<RectTransform>();
        priceRect.sizeDelta = new Vector2(50f, 20f);
        _priceText = priceObj.AddComponent<TextMeshProUGUI>();
        _priceText.text = "10g";
        _priceText.fontSize = 12;
        _priceText.color = new Color(1f, 0.85f, 0.3f);
        _priceText.alignment = TextAlignmentOptions.Left;

        // Productivity text
        GameObject prodObj = new GameObject("Productivity");
        prodObj.transform.SetParent(statsObj.transform, false);
        RectTransform prodRect = prodObj.AddComponent<RectTransform>();
        prodRect.sizeDelta = new Vector2(50f, 20f);
        _productivityText = prodObj.AddComponent<TextMeshProUGUI>();
        _productivityText.text = "+1.0";
        _productivityText.fontSize = 12;
        _productivityText.color = new Color(0.7f, 0.9f, 0.7f);
        _productivityText.alignment = TextAlignmentOptions.Left;
    }

    private void CreateBuyButton(Transform parent, float width, float height)
    {
        GameObject buttonObj = new GameObject("BuyButton");
        buttonObj.transform.SetParent(parent, false);

        RectTransform buttonRect = buttonObj.AddComponent<RectTransform>();
        buttonRect.sizeDelta = new Vector2(width, height);

        Image buttonBg = buttonObj.AddComponent<Image>();
        buttonBg.color = new Color(0.2f, 0.6f, 0.3f, 1f);

        _buyButton = buttonObj.AddComponent<Button>();
        _buyButton.targetGraphic = buttonBg;
        _buyButton.onClick.AddListener(HandleBuyClick);

        ColorBlock colors = _buyButton.colors;
        colors.normalColor = new Color(0.2f, 0.6f, 0.3f, 1f);
        colors.highlightedColor = new Color(0.3f, 0.7f, 0.4f, 1f);
        colors.pressedColor = new Color(0.15f, 0.5f, 0.25f, 1f);
        colors.disabledColor = new Color(0.3f, 0.3f, 0.3f, 0.5f);
        _buyButton.colors = colors;

        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform, false);

        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;

        _buyButtonText = textObj.AddComponent<TextMeshProUGUI>();
        _buyButtonText.text = "BUY";
        _buyButtonText.fontSize = 14;
        _buyButtonText.fontStyle = FontStyles.Bold;
        _buyButtonText.color = Color.white;
        _buyButtonText.alignment = TextAlignmentOptions.Center;
    }

    private void CreateLockedOverlay(Transform parent, float width, float height)
    {
        _lockedOverlay = new GameObject("LockedOverlay");
        _lockedOverlay.transform.SetParent(parent, false);

        RectTransform overlayRect = _lockedOverlay.AddComponent<RectTransform>();
        overlayRect.anchorMin = Vector2.zero;
        overlayRect.anchorMax = Vector2.one;
        overlayRect.sizeDelta = Vector2.zero;
        overlayRect.offsetMin = Vector2.zero;
        overlayRect.offsetMax = Vector2.zero;

        Image overlayBg = _lockedOverlay.AddComponent<Image>();
        overlayBg.color = new Color(0.1f, 0.1f, 0.1f, 0.8f);

        GameObject lockTextObj = new GameObject("LockText");
        lockTextObj.transform.SetParent(_lockedOverlay.transform, false);

        RectTransform lockTextRect = lockTextObj.AddComponent<RectTransform>();
        lockTextRect.anchorMin = Vector2.zero;
        lockTextRect.anchorMax = Vector2.one;
        lockTextRect.sizeDelta = Vector2.zero;

        _lockedText = lockTextObj.AddComponent<TextMeshProUGUI>();
        _lockedText.text = "LOCKED";
        _lockedText.fontSize = 16;
        _lockedText.fontStyle = FontStyles.Bold;
        _lockedText.color = new Color(0.5f, 0.5f, 0.5f);
        _lockedText.alignment = TextAlignmentOptions.Center;

        _lockedOverlay.SetActive(false);
    }

    private void HandleBuyClick()
    {
        OnBuyClicked?.Invoke(this);
    }

    public void SetStock(WorkhorseType type)
    {
        _stockedType = type;
        _isEmpty = false;

        _nameText.text = type.ToString();
        _iconImage.color = GameSettings.WorkhorseColors[type];

        int price = GameSettings.WorkhorsePrices[type];
        _priceText.text = $"{price}g";

        float productivity = GameSettings.WorkhorseProductivityRates[type];
        _productivityText.text = $"+{productivity:F1}";

        UpdateBuyButtonState();
    }

    public void SetEmpty()
    {
        _stockedType = null;
        _isEmpty = true;

        _nameText.text = "---";
        _iconImage.color = new Color(0.3f, 0.3f, 0.3f);
        _priceText.text = "";
        _productivityText.text = "";
        _buyButton.interactable = false;
    }

    public void SetLocked(bool locked)
    {
        _isLocked = locked;
        _lockedOverlay.SetActive(locked);

        if (locked)
        {
            _buyButton.interactable = false;
        }
        else
        {
            UpdateBuyButtonState();
        }
    }

    public void UpdateBuyButtonState()
    {
        if (_isLocked || _isEmpty || !_stockedType.HasValue)
        {
            _buyButton.interactable = false;
            return;
        }

        int price = GameSettings.WorkhorsePrices[_stockedType.Value];
        _buyButton.interactable = PlayerProgress.Instance.CanAfford(price);
    }
}
