using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class SynergyPanel : MonoBehaviour
{
    private Image _background;
    private TextMeshProUGUI _titleText;
    private List<SynergyRow> _rows = new();
    private GameObject _tooltip;
    private TextMeshProUGUI _tooltipText;
    private RectTransform _tooltipRect;
    private bool _tooltipVisible;

    private static readonly Color ActiveColor = new Color(0.2f, 0.8f, 0.2f);
    private static readonly Color InactiveColor = new Color(0.5f, 0.5f, 0.5f);
    private static readonly Color ActiveBullet = new Color(0.3f, 1f, 0.3f);
    private static readonly Color InactiveBullet = new Color(0.4f, 0.4f, 0.4f);

    public static SynergyPanel Create(Transform parent, float width, float height)
    {
        GameObject obj = new GameObject("SynergyPanel");
        obj.transform.SetParent(parent, false);

        SynergyPanel component = obj.AddComponent<SynergyPanel>();
        component.BuildUI(obj, width, height);

        return component;
    }

    private void Update()
    {
        UpdateDisplay();
        if (_tooltipVisible)
        {
            UpdateTooltipPosition();
        }
    }

    private void BuildUI(GameObject root, float width, float height)
    {
        RectTransform rect = root.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0f, 0f);
        rect.anchorMax = new Vector2(0f, 0f);
        rect.pivot = new Vector2(0f, 0f);
        rect.sizeDelta = new Vector2(width, height);
        rect.anchoredPosition = new Vector2(10f, 10f);

        _background = root.AddComponent<Image>();
        _background.sprite = SpriteLoader.Instance.GetSprite("Sprites/UIUX/ComboList");
        _background.type = Image.Type.Simple;
        _background.color = Color.white;       // Use white to show sprite's true colors

        CreateTitle(root.transform, width);
        CreateSynergyRows(root.transform, width);
        CreateTooltip(root.transform);
    }

    private void CreateTitle(Transform parent, float width)
    {
        GameObject titleObj = new GameObject("Title");
        titleObj.transform.SetParent(parent, false);

        RectTransform titleRect = titleObj.AddComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0f, 1f);
        titleRect.anchorMax = new Vector2(1f, 1f);
        titleRect.pivot = new Vector2(0.5f, 1f);
        titleRect.sizeDelta = new Vector2(0f, 25f);
        titleRect.anchoredPosition = new Vector2(0f, -20f);

        _titleText = titleObj.AddComponent<TextMeshProUGUI>();
        _titleText.text = "SYNERGIES";
        _titleText.fontSize = 14;
        _titleText.fontStyle = FontStyles.Bold;
        _titleText.color = new Color(1f, 0.9f, 0.5f);
        _titleText.alignment = TextAlignmentOptions.Center;
    }

    private void CreateSynergyRows(Transform parent, float width)
    {
        float rowHeight = 18f;
        float startY = -48f; // Below title with 20px top padding
        float currentY = startY;

        // Ability synergies section (workhorse-specific abilities)
        if (SynergySystem.Instance.AbilitySynergies.Count > 0)
        {
            CreateSectionHeader(parent, "Abilities", ref currentY);
            foreach (var synergy in SynergySystem.Instance.AbilitySynergies)
            {
                CreateRow(parent, synergy, width, rowHeight, ref currentY);
            }
            currentY -= 5f;
        }

        // Global synergies section
        if (SynergySystem.Instance.GlobalSynergies.Count > 0)
        {
            CreateSectionHeader(parent, "Global", ref currentY);
            foreach (var synergy in SynergySystem.Instance.GlobalSynergies)
            {
                CreateRow(parent, synergy, width, rowHeight, ref currentY);
            }
            currentY -= 5f;
        }

        // Adjacent synergies section
        if (SynergySystem.Instance.AdjacentSynergies.Count > 0)
        {
            CreateSectionHeader(parent, "Adjacent", ref currentY);
            foreach (var synergy in SynergySystem.Instance.AdjacentSynergies)
            {
                CreateRow(parent, synergy, width, rowHeight, ref currentY);
            }
            currentY -= 5f;
        }

        // Position synergies section
        if (SynergySystem.Instance.PositionSynergies.Count > 0)
        {
            CreateSectionHeader(parent, "Position", ref currentY);
            foreach (var synergy in SynergySystem.Instance.PositionSynergies)
            {
                CreateRow(parent, synergy, width, rowHeight, ref currentY);
            }
        }
    }

    private void CreateSectionHeader(Transform parent, string sectionName, ref float currentY)
    {
        GameObject headerObj = new GameObject($"Header_{sectionName}");
        headerObj.transform.SetParent(parent, false);

        RectTransform headerRect = headerObj.AddComponent<RectTransform>();
        headerRect.anchorMin = new Vector2(0f, 1f);
        headerRect.anchorMax = new Vector2(1f, 1f);
        headerRect.pivot = new Vector2(0f, 1f);
        headerRect.sizeDelta = new Vector2(0f, 14f);
        headerRect.anchoredPosition = new Vector2(25f, currentY);

        TextMeshProUGUI headerText = headerObj.AddComponent<TextMeshProUGUI>();
        headerText.text = sectionName;
        headerText.fontSize = 10;
        headerText.fontStyle = FontStyles.Italic;
        headerText.color = new Color(0.7f, 0.7f, 0.8f);
        headerText.alignment = TextAlignmentOptions.Left;

        currentY -= 16f;
    }

    private void CreateTooltip(Transform parent)
    {
        _tooltip = new GameObject("Tooltip");
        _tooltip.transform.SetParent(parent, false);

        _tooltipRect = _tooltip.AddComponent<RectTransform>();
        _tooltipRect.anchorMin = new Vector2(0f, 0f);
        _tooltipRect.anchorMax = new Vector2(0f, 0f);
        _tooltipRect.pivot = new Vector2(0f, 1f);
        _tooltipRect.sizeDelta = new Vector2(200f, 60f);

        Image tooltipBg = _tooltip.AddComponent<Image>();
        tooltipBg.color = new Color(0.08f, 0.08f, 0.12f, 0.95f);

        // Text inside tooltip
        GameObject textObj = new GameObject("TooltipText");
        textObj.transform.SetParent(_tooltip.transform, false);

        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;
        textRect.offsetMin = new Vector2(8f, 5f);
        textRect.offsetMax = new Vector2(-8f, -5f);

        _tooltipText = textObj.AddComponent<TextMeshProUGUI>();
        _tooltipText.fontSize = 22;
        _tooltipText.fontStyle = FontStyles.Italic;
        _tooltipText.color = new Color(0.85f, 0.85f, 0.9f);
        _tooltipText.alignment = TextAlignmentOptions.Left;
        _tooltipText.enableWordWrapping = true;

        _tooltip.SetActive(false);
    }

    private void ShowTooltip(Synergy synergy, RectTransform rowRect)
    {
        // For ability synergies, show just the description; for others show bonus + description
        if (synergy.Type == SynergyType.WorkhorseAbility)
        {
            _tooltipText.text = synergy.Description;
        }
        else
        {
            _tooltipText.text = $"+{synergy.BonusPercent:F0}% Productivity\n{synergy.Description}";
        }

        // Force layout update to get proper text bounds
        _tooltipText.ForceMeshUpdate();
        Vector2 textSize = _tooltipText.GetPreferredValues();
        float tooltipWidth = Mathf.Min(textSize.x + 16f, 220f);
        float tooltipHeight = _tooltipText.GetPreferredValues(tooltipWidth - 16f, 0f).y + 10f;
        _tooltipRect.sizeDelta = new Vector2(tooltipWidth, tooltipHeight);

        _tooltipVisible = true;
        _tooltip.SetActive(true);
        UpdateTooltipPosition();
    }

    private void UpdateTooltipPosition()
    {
        if (Mouse.current == null) return;

        // Convert mouse position to local position within the panel
        RectTransform panelRect = GetComponent<RectTransform>();
        Vector2 mousePos = Mouse.current.position.ReadValue();
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            panelRect,
            mousePos,
            null,
            out localPoint
        );

        // Offset tooltip to appear to the right and slightly above the cursor
        float offsetX = 15f;
        float offsetY = 10f;
        _tooltipRect.anchoredPosition = new Vector2(localPoint.x + offsetX, localPoint.y + offsetY);
    }

    private void HideTooltip()
    {
        _tooltipVisible = false;
        _tooltip.SetActive(false);
    }

    private void CreateRow(Transform parent, Synergy synergy, float width, float height, ref float currentY)
    {
        GameObject rowObj = new GameObject($"Row_{synergy.Name}");
        rowObj.transform.SetParent(parent, false);

        RectTransform rowRect = rowObj.AddComponent<RectTransform>();
        rowRect.anchorMin = new Vector2(0f, 1f);
        rowRect.anchorMax = new Vector2(1f, 1f);
        rowRect.pivot = new Vector2(0f, 1f);
        rowRect.sizeDelta = new Vector2(0f, height);
        rowRect.anchoredPosition = new Vector2(0f, currentY);

        // Bullet
        GameObject bulletObj = new GameObject("Bullet");
        bulletObj.transform.SetParent(rowObj.transform, false);

        RectTransform bulletRect = bulletObj.AddComponent<RectTransform>();
        bulletRect.anchorMin = new Vector2(0f, 0.5f);
        bulletRect.anchorMax = new Vector2(0f, 0.5f);
        bulletRect.pivot = new Vector2(0.5f, 0.5f);
        bulletRect.sizeDelta = new Vector2(10f, 10f);
        bulletRect.anchoredPosition = new Vector2(32f, 0f);

        Image bulletImage = bulletObj.AddComponent<Image>();
        bulletImage.raycastTarget = false;
        bulletImage.color = InactiveBullet;

        // Text
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(rowObj.transform, false);

        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = new Vector2(0f, 0f);
        textRect.anchorMax = new Vector2(1f, 1f);
        textRect.sizeDelta = Vector2.zero;
        textRect.offsetMin = new Vector2(42f, 0f);
        textRect.offsetMax = new Vector2(-25f, 0f);

        TextMeshProUGUI text = textObj.AddComponent<TextMeshProUGUI>();
        text.raycastTarget = false;
        // For ability synergies, show just the name; for others show name + bonus
        if (synergy.Type == SynergyType.WorkhorseAbility)
        {
            text.text = synergy.Name;
        }
        else
        {
            text.text = $"{synergy.Name} (+{synergy.BonusPercent:F0}%)";
        }
        text.fontSize = 11;
        text.color = InactiveColor;
        text.alignment = TextAlignmentOptions.Left;

        var row = new SynergyRow
        {
            Synergy = synergy,
            BulletImage = bulletImage,
            NameText = text,
            RowRect = rowRect
        };
        _rows.Add(row);

        // Add hover detection
        EventTrigger eventTrigger = rowObj.AddComponent<EventTrigger>();

        EventTrigger.Entry enterEntry = new EventTrigger.Entry();
        enterEntry.eventID = EventTriggerType.PointerEnter;
        enterEntry.callback.AddListener(_ => ShowTooltip(row.Synergy, row.RowRect));
        eventTrigger.triggers.Add(enterEntry);

        EventTrigger.Entry exitEntry = new EventTrigger.Entry();
        exitEntry.eventID = EventTriggerType.PointerExit;
        exitEntry.callback.AddListener(_ => HideTooltip());
        eventTrigger.triggers.Add(exitEntry);

        // Add transparent raycast target for hover detection
        Image rowBg = rowObj.AddComponent<Image>();
        rowBg.color = Color.clear;
        rowBg.raycastTarget = true;

        currentY -= height;
    }

    private void UpdateDisplay()
    {
        var synergyResults = TurnManager.Instance.PreviewSynergies();
        var activeSet = new HashSet<string>();

        foreach (var result in synergyResults)
        {
            if (result.IsActive)
            {
                activeSet.Add(result.Synergy.Name);
            }
        }

        foreach (var row in _rows)
        {
            bool isActive = activeSet.Contains(row.Synergy.Name);
            row.BulletImage.color = isActive ? ActiveBullet : InactiveBullet;
            row.NameText.color = isActive ? ActiveColor : InactiveColor;
        }
    }

    private class SynergyRow
    {
        public Synergy Synergy;
        public Image BulletImage;
        public TextMeshProUGUI NameText;
        public RectTransform RowRect;
    }
}
