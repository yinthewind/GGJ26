using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class WorkspaceHoverTooltip : MonoBehaviour
{
    private GameObject _tooltipPanel;
    private TextMeshProUGUI _titleText;
    private TextMeshProUGUI _descriptionText;
    private RectTransform _tooltipRect;
    private Canvas _canvas;
    private Camera _mainCamera;

    private Vector2Int? _previousHoveredGridPos;
    private bool _isVisible;

    private const float TooltipWidth = 220f;
    private const float TooltipPadding = 12f;
    private const float MouseOffset = 15f;

    public static WorkspaceHoverTooltip Create(Transform parent)
    {
        GameObject obj = new GameObject("WorkspaceHoverTooltip");
        obj.transform.SetParent(parent, false);

        WorkspaceHoverTooltip component = obj.AddComponent<WorkspaceHoverTooltip>();
        component.BuildUI(obj);

        return component;
    }

    private void Awake()
    {
        _mainCamera = Camera.main;
        _canvas = GetComponentInParent<Canvas>();
    }

    private void Update()
    {
        UpdateHover();
    }

    private void BuildUI(GameObject root)
    {
        RectTransform rootRect = root.AddComponent<RectTransform>();
        rootRect.anchorMin = Vector2.zero;
        rootRect.anchorMax = Vector2.zero;
        rootRect.pivot = new Vector2(0f, 1f);

        _tooltipPanel = new GameObject("TooltipPanel");
        _tooltipPanel.transform.SetParent(root.transform, false);

        _tooltipRect = _tooltipPanel.AddComponent<RectTransform>();
        _tooltipRect.anchorMin = Vector2.zero;
        _tooltipRect.anchorMax = Vector2.zero;
        _tooltipRect.pivot = new Vector2(0f, 1f);
        _tooltipRect.sizeDelta = new Vector2(TooltipWidth, 80f);

        Image panelBg = _tooltipPanel.AddComponent<Image>();
        panelBg.color = new Color(0.1f, 0.1f, 0.15f, 0.95f);

        VerticalLayoutGroup layout = _tooltipPanel.AddComponent<VerticalLayoutGroup>();
        layout.padding = new RectOffset((int)TooltipPadding, (int)TooltipPadding, (int)TooltipPadding, (int)TooltipPadding);
        layout.spacing = 4f;
        layout.childAlignment = TextAnchor.UpperLeft;
        layout.childControlWidth = true;
        layout.childControlHeight = true;
        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = false;

        ContentSizeFitter fitter = _tooltipPanel.AddComponent<ContentSizeFitter>();
        fitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        CreateTitleText(_tooltipPanel.transform);
        CreateDescriptionText(_tooltipPanel.transform);

        Hide();
    }

    private void CreateTitleText(Transform parent)
    {
        GameObject titleObj = new GameObject("Title");
        titleObj.transform.SetParent(parent, false);

        _titleText = titleObj.AddComponent<TextMeshProUGUI>();
        _titleText.fontSize = 16;
        _titleText.fontStyle = FontStyles.Bold;
        _titleText.color = new Color(1f, 0.9f, 0.5f);
        _titleText.alignment = TextAlignmentOptions.Left;

        LayoutElement titleLayout = titleObj.AddComponent<LayoutElement>();
        titleLayout.preferredWidth = TooltipWidth - TooltipPadding * 2;
    }

    private void CreateDescriptionText(Transform parent)
    {
        GameObject descObj = new GameObject("Description");
        descObj.transform.SetParent(parent, false);

        _descriptionText = descObj.AddComponent<TextMeshProUGUI>();
        _descriptionText.fontSize = 13;
        _descriptionText.fontStyle = FontStyles.Italic;
        _descriptionText.color = new Color(0.8f, 0.8f, 0.85f);
        _descriptionText.alignment = TextAlignmentOptions.Left;
        _descriptionText.enableWordWrapping = true;

        LayoutElement descLayout = descObj.AddComponent<LayoutElement>();
        descLayout.preferredWidth = TooltipWidth - TooltipPadding * 2;
    }

    private void UpdateHover()
    {
        if (ShouldHideTooltip())
        {
            ClearHoverState();
            return;
        }

        var mouse = Mouse.current;
        if (mouse == null) return;

        Vector2 mouseScreenPos = mouse.position.ReadValue();
        Vector3 mouseWorldPos = _mainCamera.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, -_mainCamera.transform.position.z));

        Vector2Int? gridPosResult = GridSystem.RaycastToGridPosition(mouseWorldPos);

        if (!gridPosResult.HasValue)
        {
            ClearHoverState();
            return;
        }

        Vector2Int gridPos = gridPosResult.Value;

        if (_previousHoveredGridPos.HasValue && _previousHoveredGridPos.Value == gridPos)
        {
            if (_isVisible)
            {
                UpdateTooltipPosition(mouseScreenPos);
            }
            return;
        }

        ClearHighlight();

        WorkspaceController workspace = WorkspaceControllers.Instance.GetWorkspaceAtGridPosition(gridPos);
        if (workspace == null || !workspace.IsOccupied)
        {
            _previousHoveredGridPos = gridPos;
            Hide();
            return;
        }

        WorkhorseController workhorse = CharacterControllers.Instance.GetByEntityId(workspace.AssignedSkeletonId.Value);
        if (workhorse == null || !workhorse.IsRevealed)
        {
            _previousHoveredGridPos = gridPos;
            Hide();
            return;
        }

        FloorGridAnimatorManager.SetHighlight(gridPos, HighlightType.Hover);
        _previousHoveredGridPos = gridPos;

        ShowTooltipForWorkhorse(workhorse, mouseScreenPos);
    }

    private bool ShouldHideTooltip()
    {
        if (ModalManager.Instance != null && ModalManager.Instance.HasModal)
            return true;

        if (CheckModeManager.Instance != null && CheckModeManager.Instance.IsCheckModeActive)
            return true;

        if (DragDropInputSystem.Instance != null &&
            (DragDropInputSystem.Instance.IsPlacingWorkspace || DragDropInputSystem.Instance.IsDragging))
            return true;

        return false;
    }

    private void ClearHoverState()
    {
        ClearHighlight();
        _previousHoveredGridPos = null;
        Hide();
    }

    private void ClearHighlight()
    {
        if (_previousHoveredGridPos.HasValue)
        {
            FloorGridAnimatorManager.ClearHighlight(_previousHoveredGridPos.Value);
        }
    }

    private void ShowTooltipForWorkhorse(WorkhorseController workhorse, Vector2 mouseScreenPos)
    {
        string title = GetWorkhorseDisplayName(workhorse.Type);
        string description = GetWorkhorseDescription(workhorse.Type);

        _titleText.text = title;
        _descriptionText.text = description;

        LayoutRebuilder.ForceRebuildLayoutImmediate(_tooltipRect);

        UpdateTooltipPosition(mouseScreenPos);
        Show();
    }

    private void UpdateTooltipPosition(Vector2 mouseScreenPos)
    {
        if (_canvas == null) return;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _canvas.GetComponent<RectTransform>(),
            mouseScreenPos,
            _canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : _canvas.worldCamera,
            out Vector2 localPoint);

        float tooltipHeight = _tooltipRect.sizeDelta.y;
        float tooltipWidth = _tooltipRect.sizeDelta.x;

        float xPos = localPoint.x + MouseOffset;
        float yPos = localPoint.y + MouseOffset;

        RectTransform canvasRect = _canvas.GetComponent<RectTransform>();
        float canvasWidth = canvasRect.rect.width;
        float canvasHeight = canvasRect.rect.height;

        if (xPos + tooltipWidth > canvasWidth / 2)
        {
            xPos = localPoint.x - tooltipWidth - MouseOffset;
        }

        if (yPos > canvasHeight / 2)
        {
            yPos = localPoint.y - tooltipHeight - MouseOffset;
            _tooltipRect.pivot = new Vector2(0f, 0f);
        }
        else
        {
            _tooltipRect.pivot = new Vector2(0f, 1f);
        }

        _tooltipRect.anchoredPosition = new Vector2(xPos, yPos);
    }

    private string GetWorkhorseDisplayName(WorkhorseType type)
    {
        return type switch
        {
            WorkhorseType.InternNiuma => "Intern Niuma",
            WorkhorseType.RegularNiuma => "Regular Niuma",
            WorkhorseType.SuperNiuma => "Super Niuma",
            WorkhorseType.ToxicWolf => "Toxic Wolf",
            WorkhorseType.Encourager => "Encourager",
            WorkhorseType.RisingStar => "Rising Star",
            WorkhorseType.FreeSpirit => "Free Spirit",
            WorkhorseType.Pessimist => "Pessimist",
            WorkhorseType.Saboteur => "Saboteur",
            _ => type.ToString()
        };
    }

    private string GetWorkhorseDescription(WorkhorseType type)
    {
        if (IsAbilityType(type))
        {
            foreach (var synergy in SynergySystem.Instance.AbilitySynergies)
            {
                if (synergy is AbilitySynergy abilitySynergy && abilitySynergy.WorkhorseType == type)
                {
                    return synergy.Description;
                }
            }
        }

        float baseRate = GameSettings.WorkhorseProductivityRates[type];
        int baseValue = Mathf.RoundToInt(baseRate * 100);
        return $"Base productivity: {baseValue}";
    }

    private bool IsAbilityType(WorkhorseType type)
    {
        return type != WorkhorseType.InternNiuma &&
               type != WorkhorseType.RegularNiuma &&
               type != WorkhorseType.SuperNiuma;
    }

    private void Show()
    {
        _tooltipPanel.SetActive(true);
        _isVisible = true;
    }

    private void Hide()
    {
        _tooltipPanel.SetActive(false);
        _isVisible = false;
    }
}
