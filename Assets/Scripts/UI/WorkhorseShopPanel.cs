using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WorkhorseShopPanel : MonoBehaviour
{
    public static WorkhorseShopPanel Instance { get; private set; }

    private RectTransform _rectTransform;
    private Image _background;
    private TextMeshProUGUI _titleText;
    private TextMeshProUGUI _dollarText;
    private List<WorkhorseShopSlot> _slots = new();
    private Button _refreshButton;
    private TextMeshProUGUI _refreshButtonText;
    private WorkhorseFireZone _fireZone;
    private WorkspaceShopSlot _workspaceSlot;

    private int _totalSlots;
    private int _activeSlots;
    private bool _freeRefreshAvailable = true;
    private bool _workspaceSoldOutThisTurn = false;

    public WorkhorseFireZone FireZone => _fireZone;

    private static readonly Vector3 SpawnPosition = new Vector3(0f, 5f, 0f);

    public static WorkhorseShopPanel Create(Transform parent, float width, float height, int totalSlots, int activeSlots)
    {
        GameObject obj = new GameObject("WorkhorseShopPanel");
        obj.transform.SetParent(parent, false);

        WorkhorseShopPanel component = obj.AddComponent<WorkhorseShopPanel>();
        component._totalSlots = totalSlots;
        component._activeSlots = activeSlots;
        component.BuildUI(obj, width, height);
        component.RefreshShop();

        return component;
    }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        PlayerProgress.Instance.OnDollarChanged += HandleDollarChanged;
        TurnManager.Instance.OnTurnStarted += HandleTurnStarted;

        var inputSystem = FindObjectOfType<DragDropInputSystem>();
        if (inputSystem != null)
            inputSystem.OnWorkspacePlaced += HandleWorkspacePlaced;

        UpdateDollarDisplay();
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;

        PlayerProgress.Instance.OnDollarChanged -= HandleDollarChanged;
        TurnManager.Instance.OnTurnStarted -= HandleTurnStarted;

        var inputSystem = FindObjectOfType<DragDropInputSystem>();
        if (inputSystem != null)
            inputSystem.OnWorkspacePlaced -= HandleWorkspacePlaced;
    }

    private void BuildUI(GameObject root, float width, float height)
    {
        _rectTransform = root.AddComponent<RectTransform>();
        _rectTransform.anchorMin = new Vector2(1f, 0.5f);
        _rectTransform.anchorMax = new Vector2(1f, 0.5f);
        _rectTransform.pivot = new Vector2(1f, 0.5f);
        _rectTransform.sizeDelta = new Vector2(width, height);
        _rectTransform.anchoredPosition = new Vector2(-10f, 0f);

        _background = root.AddComponent<Image>();
        _background.color = new Color(0.1f, 0.1f, 0.15f, 0.95f);

        // Vertical layout for the whole panel
        VerticalLayoutGroup mainLayout = root.AddComponent<VerticalLayoutGroup>();
        mainLayout.padding = new RectOffset(10, 10, 10, 10);
        mainLayout.spacing = 8f;
        mainLayout.childAlignment = TextAnchor.UpperCenter;
        mainLayout.childControlWidth = true;
        mainLayout.childControlHeight = false;
        mainLayout.childForceExpandWidth = true;
        mainLayout.childForceExpandHeight = false;

        float innerWidth = width - 20f;

        CreateHeader(root.transform, innerWidth, 50f);
        CreateSlots(root.transform, innerWidth, 45f);
        CreateRefreshButton(root.transform, innerWidth, 35f);
        CreateDivider(root.transform, innerWidth);
        CreateWorkspaceSection(root.transform, innerWidth, 45f);
        CreateFireZone(root.transform, innerWidth, 80f);
    }

    private void CreateHeader(Transform parent, float width, float height)
    {
        GameObject headerObj = new GameObject("Header");
        headerObj.transform.SetParent(parent, false);

        RectTransform headerRect = headerObj.AddComponent<RectTransform>();
        headerRect.sizeDelta = new Vector2(width, height);

        VerticalLayoutGroup headerLayout = headerObj.AddComponent<VerticalLayoutGroup>();
        headerLayout.childAlignment = TextAnchor.MiddleCenter;
        headerLayout.childControlWidth = true;
        headerLayout.childControlHeight = true;
        headerLayout.childForceExpandWidth = true;
        headerLayout.childForceExpandHeight = true;

        // Title
        GameObject titleObj = new GameObject("Title");
        titleObj.transform.SetParent(headerObj.transform, false);
        _titleText = titleObj.AddComponent<TextMeshProUGUI>();
        _titleText.text = "WORKHORSE SHOP";
        _titleText.fontSize = 18;
        _titleText.fontStyle = FontStyles.Bold;
        _titleText.color = Color.white;
        _titleText.alignment = TextAlignmentOptions.Center;

        // Dollar display
        GameObject dollarObj = new GameObject("Dollar");
        dollarObj.transform.SetParent(headerObj.transform, false);
        _dollarText = dollarObj.AddComponent<TextMeshProUGUI>();
        _dollarText.text = "$50";
        _dollarText.fontSize = 16;
        _dollarText.color = new Color(0.3f, 0.85f, 0.3f);
        _dollarText.alignment = TextAlignmentOptions.Center;
    }

    private void CreateSlots(Transform parent, float width, float slotHeight)
    {
        for (int i = 0; i < _totalSlots; i++)
        {
            WorkhorseShopSlot slot = WorkhorseShopSlot.Create(parent, width, slotHeight);
            slot.OnBuyClicked += HandleSlotBuyClicked;
            slot.OnSlotClicked += HandleSlotClicked;

            // Initialize as locked if beyond active slots
            bool isLocked = i >= _activeSlots;
            slot.SetLocked(isLocked);

            _slots.Add(slot);
        }
    }

    private void CreateRefreshButton(Transform parent, float width, float height)
    {
        GameObject buttonObj = new GameObject("RefreshButton");
        buttonObj.transform.SetParent(parent, false);

        RectTransform buttonRect = buttonObj.AddComponent<RectTransform>();
        buttonRect.sizeDelta = new Vector2(width, height);

        Image buttonBg = buttonObj.AddComponent<Image>();
        buttonBg.color = new Color(0.3f, 0.4f, 0.6f, 1f);

        _refreshButton = buttonObj.AddComponent<Button>();
        _refreshButton.targetGraphic = buttonBg;
        _refreshButton.onClick.AddListener(HandleRefreshClick);

        ColorBlock colors = _refreshButton.colors;
        colors.normalColor = new Color(0.3f, 0.4f, 0.6f, 1f);
        colors.highlightedColor = new Color(0.4f, 0.5f, 0.7f, 1f);
        colors.pressedColor = new Color(0.25f, 0.35f, 0.55f, 1f);
        colors.disabledColor = new Color(0.3f, 0.3f, 0.3f, 0.5f);
        _refreshButton.colors = colors;

        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform, false);

        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;

        _refreshButtonText = textObj.AddComponent<TextMeshProUGUI>();
        _refreshButtonText.fontSize = 14;
        _refreshButtonText.fontStyle = FontStyles.Bold;
        _refreshButtonText.color = Color.white;
        _refreshButtonText.alignment = TextAlignmentOptions.Center;

        UpdateRefreshButtonText();
    }

    private void CreateFireZone(Transform parent, float width, float height)
    {
        _fireZone = WorkhorseFireZone.Create(parent, width, height);
    }

    private void CreateDivider(Transform parent, float width)
    {
        GameObject divider = new GameObject("Divider");
        divider.transform.SetParent(parent, false);
        RectTransform rect = divider.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(width, 2f);
        Image img = divider.AddComponent<Image>();
        img.color = new Color(0.4f, 0.4f, 0.5f, 0.8f);
    }

    private void CreateWorkspaceSection(Transform parent, float width, float height)
    {
        // Section header
        GameObject headerObj = new GameObject("WorkspaceHeader");
        headerObj.transform.SetParent(parent, false);
        RectTransform headerRect = headerObj.AddComponent<RectTransform>();
        headerRect.sizeDelta = new Vector2(width, 20f);
        var headerText = headerObj.AddComponent<TextMeshProUGUI>();
        headerText.text = "WORKSPACE";
        headerText.fontSize = 14;
        headerText.fontStyle = FontStyles.Bold;
        headerText.color = Color.white;
        headerText.alignment = TextAlignmentOptions.Center;

        // Workspace slot
        _workspaceSlot = WorkspaceShopSlot.Create(parent, width, height);
        _workspaceSlot.OnBuyClicked += HandleWorkspaceBuyClicked;
    }

    private void HandleWorkspaceBuyClicked(WorkspaceShopSlot slot)
    {
        if (_workspaceSoldOutThisTurn) return;
        if (!PlayerProgress.Instance.CanAfford(GameSettings.WorkspacePrice)) return;

        // Enter placement mode
        var inputSystem = FindObjectOfType<DragDropInputSystem>();
        inputSystem.EnterWorkspacePlacementMode();
    }

    private void HandleWorkspacePlaced()
    {
        _workspaceSoldOutThisTurn = true;
        _workspaceSlot.SetSoldOut(true);
    }

    private void HandleSlotClicked(WorkhorseShopSlot slot)
    {
        if (!CheckModeManager.Instance.IsCheckModeActive) return;
        if (slot.IsRevealed || slot.IsEmpty || slot.IsLocked) return;
        if (slot.Workhorse == null) return;

        if (PlayerProgress.Instance.TrySpendDollar(GameSettings.RevealCost))
        {
            slot.Workhorse.Reveal();  // Reuse existing reveal logic
            slot.RefreshDisplay();    // Update UI to show revealed info
        }
    }

    private void HandleSlotBuyClicked(WorkhorseShopSlot slot)
    {
        if (slot.Workhorse == null || slot.IsLocked || slot.IsEmpty)
            return;

        int price = GameSettings.ShopWorkhorsePrice;

        if (!PlayerProgress.Instance.TrySpendDollar(price))
            return;

        // Enable and position the existing workhorse (already has correct reveal state)
        WorkhorseController workhorse = slot.Workhorse;
        workhorse.Transform.gameObject.SetActive(true);
        workhorse.SetPosition(SpawnPosition);
        workhorse.StartFallingIfAboveGround();

        // Clear slot without destroying workhorse
        slot.ClearWithoutDestroy();
    }

    private void HandleRefreshClick()
    {
        if (_freeRefreshAvailable)
        {
            _freeRefreshAvailable = false;
            RefreshShop();
            UpdateRefreshButtonText();
            return;
        }

        if (PlayerProgress.Instance.TrySpendDollar(GameSettings.ShopRefreshCost))
        {
            RefreshShop();
        }
    }

    private void HandleDollarChanged(int newDollar)
    {
        UpdateDollarDisplay();
        UpdateAllSlotBuyButtons();
        UpdateRefreshButtonState();
    }

    private void HandleTurnStarted()
    {
        _freeRefreshAvailable = true;
        UpdateRefreshButtonText();

        _workspaceSoldOutThisTurn = false;
        _workspaceSlot?.SetSoldOut(false);
    }

    public void RefreshShop()
    {
        List<WorkhorseType> availableTypes = GetAvailableWorkhorseTypes();

        for (int i = 0; i < _slots.Count; i++)
        {
            if (i < _activeSlots)
            {
                // Generate random workhorse from available types
                WorkhorseType randomType = availableTypes[UnityEngine.Random.Range(0, availableTypes.Count)];
                _slots[i].SetStock(randomType);
                _slots[i].SetLocked(false);
            }
            else
            {
                // Keep locked slots locked
                _slots[i].SetLocked(true);
            }
        }

        UpdateAllSlotBuyButtons();
    }

    private List<WorkhorseType> GetAvailableWorkhorseTypes()
    {
        var result = new List<WorkhorseType>();
        foreach (WorkhorseType type in Enum.GetValues(typeof(WorkhorseType)))
        {
            if (LevelManager.Instance.IsWorkhorseTypeAvailable(type))
            {
                result.Add(type);
            }
        }
        return result;
    }

    public void SetActiveSlots(int count)
    {
        _activeSlots = Mathf.Clamp(count, 0, _totalSlots);
        List<WorkhorseType> availableTypes = GetAvailableWorkhorseTypes();

        for (int i = 0; i < _slots.Count; i++)
        {
            bool isLocked = i >= _activeSlots;
            _slots[i].SetLocked(isLocked);

            // If we just unlocked a slot, give it stock
            if (!isLocked && _slots[i].IsEmpty)
            {
                WorkhorseType randomType = availableTypes[UnityEngine.Random.Range(0, availableTypes.Count)];
                _slots[i].SetStock(randomType);
            }
        }

        UpdateAllSlotBuyButtons();
    }

    private void UpdateDollarDisplay()
    {
        _dollarText.text = $"${PlayerProgress.Instance.CurrentDollar}";
    }

    private void UpdateAllSlotBuyButtons()
    {
        foreach (var slot in _slots)
        {
            slot.UpdateBuyButtonState();
        }
    }

    private void UpdateRefreshButtonText()
    {
        if (_freeRefreshAvailable)
        {
            _refreshButtonText.text = "REFRESH (Free)";
        }
        else
        {
            _refreshButtonText.text = $"REFRESH ({GameSettings.ShopRefreshCost}g)";
        }

        UpdateRefreshButtonState();
    }

    private void UpdateRefreshButtonState()
    {
        if (_freeRefreshAvailable)
        {
            _refreshButton.interactable = true;
        }
        else
        {
            _refreshButton.interactable = PlayerProgress.Instance.CanAfford(GameSettings.ShopRefreshCost);
        }
    }
}
