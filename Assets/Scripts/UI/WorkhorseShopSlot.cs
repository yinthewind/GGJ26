using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class WorkhorseShopSlot : MonoBehaviour, IPointerClickHandler
{
    public event Action<WorkhorseShopSlot> OnBuyClicked;
    public event Action<WorkhorseShopSlot> OnSlotClicked;

    private TextMeshProUGUI _nameText;
    private TextMeshProUGUI _priceText;
    private TextMeshProUGUI _productivityText;
    private Button _buyButton;
    private TextMeshProUGUI _buyButtonText;
    private GameObject _lockedOverlay;
    private TextMeshProUGUI _lockedText;
    private GameObject _contentContainer;
    private Image _iconImage;

    private WorkhorseController _workhorse;
    private bool _isLocked;
    private bool _isEmpty;

    public WorkhorseController Workhorse => _workhorse;
    public WorkhorseType? StockedType => _workhorse?.Type;
    public bool IsRevealed => _workhorse?.IsRevealed ?? false;
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

        // Content container for icon, info, and buy button
        _contentContainer = new GameObject("Content");
        _contentContainer.transform.SetParent(root.transform, false);

        RectTransform contentRect = _contentContainer.AddComponent<RectTransform>();
        contentRect.anchorMin = Vector2.zero;
        contentRect.anchorMax = Vector2.one;
        contentRect.sizeDelta = Vector2.zero;
        contentRect.offsetMin = Vector2.zero;
        contentRect.offsetMax = Vector2.zero;

        // Horizontal layout on content container
        HorizontalLayoutGroup layout = _contentContainer.AddComponent<HorizontalLayoutGroup>();
        layout.padding = new RectOffset(40, 40, 4, 4);
        layout.spacing = 8f;
        layout.childAlignment = TextAnchor.MiddleLeft;
        layout.childControlWidth = false;
        layout.childControlHeight = false;
        layout.childForceExpandWidth = false;
        layout.childForceExpandHeight = false;

        float iconSize = height - 8f;
        CreateIcon(_contentContainer.transform, iconSize);
        CreateInfoSection(_contentContainer.transform, width - iconSize - 80f - 32f, height - 8f);
        CreateBuyButton(_contentContainer.transform, 60f, height - 12f);
        CreateLockedOverlay(root.transform, width, height);
    }

    private void CreateIcon(Transform parent, float size)
    {
        GameObject iconObj = new GameObject("Icon");
        iconObj.transform.SetParent(parent, false);

        RectTransform iconRect = iconObj.AddComponent<RectTransform>();
        iconRect.sizeDelta = new Vector2(size, size);

        _iconImage = iconObj.AddComponent<Image>();
        _iconImage.color = Color.white;
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
        _nameText.color = Color.black;
        _nameText.alignment = TextAlignmentOptions.Left;

        _nameText.font = UiUtils.GetChineseFont();

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
        Sprite buyButtonSprite = SpriteLoader.Instance.GetSprite("Sprites/UIUX/BuyButton");
        float buttonWidth = buyButtonSprite.rect.width;
        float buttonHeight = buyButtonSprite.rect.height;

        GameObject buttonObj = new GameObject("BuyButton");
        buttonObj.transform.SetParent(parent, false);

        RectTransform buttonRect = buttonObj.AddComponent<RectTransform>();
        buttonRect.sizeDelta = new Vector2(buttonWidth, buttonHeight);

        Image buttonBg = buttonObj.AddComponent<Image>();
        buttonBg.sprite = buyButtonSprite;
        buttonBg.type = Image.Type.Simple;
        buttonBg.color = Color.white;

        _buyButton = buttonObj.AddComponent<Button>();
        _buyButton.targetGraphic = buttonBg;
        _buyButton.onClick.AddListener(HandleBuyClick);

        ColorBlock colors = _buyButton.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color(0.9f, 0.9f, 0.9f, 1f);
        colors.pressedColor = new Color(0.7f, 0.7f, 0.7f, 1f);
        colors.disabledColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        _buyButton.colors = colors;

        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform, false);

        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;

        _buyButtonText = textObj.AddComponent<TextMeshProUGUI>();
        _buyButtonText.text = "购买";
        _buyButtonText.fontSize = 14;
        _buyButtonText.fontStyle = FontStyles.Bold;
        _buyButtonText.color = Color.black;
        _buyButtonText.alignment = TextAlignmentOptions.Center;
        _buyButtonText.font = UiUtils.GetChineseFont();
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

        GameObject lockTextObj = new GameObject("LockText");
        lockTextObj.transform.SetParent(_lockedOverlay.transform, false);

        RectTransform lockTextRect = lockTextObj.AddComponent<RectTransform>();
        lockTextRect.anchorMin = Vector2.zero;
        lockTextRect.anchorMax = Vector2.one;
        lockTextRect.sizeDelta = Vector2.zero;

        _lockedText = lockTextObj.AddComponent<TextMeshProUGUI>();
        _lockedText.text = "锁定";
        _lockedText.fontSize = 16;
        _lockedText.fontStyle = FontStyles.Bold;
        _lockedText.color = new Color(0.5f, 0.5f, 0.5f);
        _lockedText.alignment = TextAlignmentOptions.Center;
        _lockedText.font = UiUtils.GetChineseFont();

        _lockedOverlay.SetActive(false);
    }

    private void HandleBuyClick()
    {
        OnBuyClicked?.Invoke(this);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnSlotClicked?.Invoke(this);
    }

    public void SetStock(WorkhorseType type)
    {
        // Destroy previous workhorse if exists
        if (_workhorse != null)
        {
            CharacterControllers.Instance.DestroySkeleton(_workhorse);
        }

        // Create workhorse but keep hidden (disabled)
        _workhorse = CharacterControllers.Instance.SpawnSkeleton(type, new Vector3(-100f, -100f, 0f));
        _workhorse.Transform.gameObject.SetActive(false);

        _isEmpty = false;
        UpdateDisplay();
    }

    public void SetEmpty()
    {
        if (_workhorse != null)
        {
            CharacterControllers.Instance.DestroySkeleton(_workhorse);
            _workhorse = null;
        }
        _isEmpty = true;
        UpdateDisplay();
    }

    public void ClearWithoutDestroy()
    {
        _workhorse = null;  // Release reference without destroying
        _isEmpty = true;
        UpdateDisplay();
    }

    public void RefreshDisplay()
    {
        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        if (_isEmpty || _workhorse == null)
        {
            _nameText.text = "---";
            _priceText.text = "";
            _productivityText.text = "";
            _buyButton.interactable = false;
            _iconImage.sprite = null;
            _iconImage.color = new Color(0.3f, 0.3f, 0.3f);
            return;
        }

        if (_workhorse.IsRevealed)
        {
            // Revealed: show actual info
            WorkhorseType type = _workhorse.Type;
            _nameText.text = WorkhorseAnimator.GetTypeAbbreviation(type);
            _productivityText.text = $"+{GameSettings.WorkhorseProductivityRates[type]:F1}";
            _iconImage.sprite = null;
            _iconImage.color = Color.white;
        }
        else
        {
            // Masked: hide info (except price)
            _nameText.text = "??";
            _productivityText.text = "??";
            _iconImage.sprite = SpriteLoader.Instance.GetSprite("Sprites/UIUX/mask_Icon");
            _iconImage.color = Color.white;
        }
        // Price is always visible
        _priceText.text = $"{GameSettings.ShopWorkhorsePrice}g";
        UpdateBuyButtonState();
    }

    public void SetLocked(bool locked)
    {
        _isLocked = locked;
        _lockedOverlay.SetActive(locked);
        _contentContainer.SetActive(!locked);

        if (!locked)
        {
            UpdateBuyButtonState();
        }
    }

    public void UpdateBuyButtonState()
    {
        if (_isLocked || _isEmpty || _workhorse == null)
        {
            _buyButton.interactable = false;
            return;
        }

        _buyButton.interactable = PlayerProgress.Instance.CanAfford(GameSettings.ShopWorkhorsePrice);
    }
}
