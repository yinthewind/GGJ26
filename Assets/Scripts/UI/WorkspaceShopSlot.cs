using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WorkspaceShopSlot : MonoBehaviour
{
    public event Action<WorkspaceShopSlot> OnBuyClicked;

    private TextMeshProUGUI _nameText;
    private TextMeshProUGUI _priceText;
    private Button _buyButton;
    private TextMeshProUGUI _buyButtonText;
    private GameObject _contentContainer;
    private GameObject _soldOutOverlay;

    private bool _isSoldOut;

    public bool IsSoldOut => _isSoldOut;

    public static WorkspaceShopSlot Create(Transform parent, float width, float height)
    {
        GameObject obj = new GameObject("WorkspaceShopSlot");
        obj.transform.SetParent(parent, false);

        WorkspaceShopSlot component = obj.AddComponent<WorkspaceShopSlot>();
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

        CreateInfoSection(_contentContainer.transform, width - 80f - 32f, height - 8f);
        CreateBuyButton(_contentContainer.transform, 60f, height - 12f);
        CreateSoldOutOverlay(root.transform, width, height);
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
        _nameText.text = "工位";
        _nameText.fontSize = 14;
        _nameText.fontStyle = FontStyles.Bold;
        _nameText.color = Color.white;
        _nameText.alignment = TextAlignmentOptions.Left;
        _nameText.font = UiUtils.GetChineseFont();

        // Price text
        GameObject priceObj = new GameObject("Price");
        priceObj.transform.SetParent(infoObj.transform, false);
        _priceText = priceObj.AddComponent<TextMeshProUGUI>();
        _priceText.text = $"{GameSettings.WorkspacePrice}g";
        _priceText.fontSize = 12;
        _priceText.color = new Color(1f, 0.85f, 0.3f);
        _priceText.alignment = TextAlignmentOptions.Left;
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
        _buyButtonText.color = Color.white;
        _buyButtonText.alignment = TextAlignmentOptions.Center;
        _buyButtonText.font = UiUtils.GetChineseFont();
    }

    private void CreateSoldOutOverlay(Transform parent, float width, float height)
    {
        _soldOutOverlay = new GameObject("SoldOutOverlay");
        _soldOutOverlay.transform.SetParent(parent, false);

        RectTransform overlayRect = _soldOutOverlay.AddComponent<RectTransform>();
        overlayRect.anchorMin = Vector2.zero;
        overlayRect.anchorMax = Vector2.one;
        overlayRect.sizeDelta = Vector2.zero;
        overlayRect.offsetMin = Vector2.zero;
        overlayRect.offsetMax = Vector2.zero;

        Image overlayBg = _soldOutOverlay.AddComponent<Image>();
        overlayBg.color = new Color(0.1f, 0.1f, 0.1f, 0.8f);

        GameObject soldOutTextObj = new GameObject("SoldOutText");
        soldOutTextObj.transform.SetParent(_soldOutOverlay.transform, false);

        RectTransform soldOutTextRect = soldOutTextObj.AddComponent<RectTransform>();
        soldOutTextRect.anchorMin = Vector2.zero;
        soldOutTextRect.anchorMax = Vector2.one;
        soldOutTextRect.sizeDelta = Vector2.zero;

        TextMeshProUGUI soldOutText = soldOutTextObj.AddComponent<TextMeshProUGUI>();
        soldOutText.text = "售罄";
        soldOutText.fontSize = 16;
        soldOutText.fontStyle = FontStyles.Bold;
        soldOutText.color = new Color(0.5f, 0.5f, 0.5f);
        soldOutText.alignment = TextAlignmentOptions.Center;
        soldOutText.font = UiUtils.GetChineseFont();

        _soldOutOverlay.SetActive(false);
    }

    private void HandleBuyClick()
    {
        OnBuyClicked?.Invoke(this);
    }

    public void SetSoldOut(bool soldOut)
    {
        _isSoldOut = soldOut;
        _soldOutOverlay.SetActive(soldOut);
        _contentContainer.SetActive(!soldOut);
    }

    public void UpdateBuyButtonState()
    {
        if (_isSoldOut)
        {
            _buyButton.interactable = false;
            return;
        }

        _buyButton.interactable = PlayerProgress.Instance.CanAfford(GameSettings.WorkspacePrice);
    }

    private void Start()
    {
        PlayerProgress.Instance.OnDollarChanged += HandleDollarChanged;
        UpdateBuyButtonState();
    }

    private void OnDestroy()
    {
        if (PlayerProgress.Instance != null)
        {
            PlayerProgress.Instance.OnDollarChanged -= HandleDollarChanged;
        }
    }

    private void HandleDollarChanged(int newDollar)
    {
        UpdateBuyButtonState();
    }
}
