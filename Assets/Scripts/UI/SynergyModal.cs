using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SynergyModal : MonoBehaviour
{
    private GameObject _overlay;
    private GameObject _panel;
    private List<SynergyRow> _rows = new();

    private static readonly Color ActiveColor = Color.black;
    private static readonly Color InactiveColor = Color.black;
    private static readonly Color ActiveBullet = new Color(0.3f, 1f, 0.3f);
    private static readonly Color InactiveBullet = new Color(0.4f, 0.4f, 0.4f);

    public static SynergyModal Create(Transform parent)
    {
        GameObject obj = new GameObject("SynergyModal");
        obj.transform.SetParent(parent, false);

        SynergyModal component = obj.AddComponent<SynergyModal>();
        component.BuildUI(obj);
        component.Hide();

        return component;
    }

    private void Update()
    {
        if (gameObject.activeSelf)
        {
            UpdateDisplay();
        }
    }

    public void Show()
    {
        gameObject.SetActive(true);
        UpdateDisplay();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public bool IsVisible => gameObject.activeSelf;

    private void BuildUI(GameObject root)
    {
        // Root fills the entire screen
        RectTransform rootRect = root.AddComponent<RectTransform>();
        rootRect.anchorMin = Vector2.zero;
        rootRect.anchorMax = Vector2.one;
        rootRect.sizeDelta = Vector2.zero;
        rootRect.offsetMin = Vector2.zero;
        rootRect.offsetMax = Vector2.zero;

        CreateOverlay(root.transform);
        CreatePanel(root.transform);
    }

    private void CreateOverlay(Transform parent)
    {
        _overlay = new GameObject("Overlay");
        _overlay.transform.SetParent(parent, false);

        RectTransform overlayRect = _overlay.AddComponent<RectTransform>();
        overlayRect.anchorMin = Vector2.zero;
        overlayRect.anchorMax = Vector2.one;
        overlayRect.sizeDelta = Vector2.zero;

        Image overlayImage = _overlay.AddComponent<Image>();
        overlayImage.color = new Color(0f, 0f, 0f, 0.7f);

        // Close when clicking overlay
        Button overlayButton = _overlay.AddComponent<Button>();
        overlayButton.transition = Selectable.Transition.None;
        overlayButton.onClick.AddListener(Hide);
    }

    private void CreatePanel(Transform parent)
    {
        _panel = new GameObject("Panel");
        _panel.transform.SetParent(parent, false);

        RectTransform panelRect = _panel.AddComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.5f, 0.5f);
        panelRect.anchorMax = new Vector2(0.5f, 0.5f);
        panelRect.pivot = new Vector2(0.5f, 0.5f);
        panelRect.sizeDelta = new Vector2(500f, 550f);

        Image panelBg = _panel.AddComponent<Image>();
        panelBg.color = new Color(0.12f, 0.12f, 0.18f, 0.98f);

        CreateTitle(_panel.transform);
        CreateCloseButton(_panel.transform);
        CreateSynergyList(_panel.transform);
    }

    private void CreateTitle(Transform parent)
    {
        GameObject titleObj = new GameObject("Title");
        titleObj.transform.SetParent(parent, false);

        RectTransform titleRect = titleObj.AddComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0f, 1f);
        titleRect.anchorMax = new Vector2(1f, 1f);
        titleRect.pivot = new Vector2(0.5f, 1f);
        titleRect.sizeDelta = new Vector2(0f, 50f);
        titleRect.anchoredPosition = Vector2.zero;

        TextMeshProUGUI titleText = titleObj.AddComponent<TextMeshProUGUI>();
        titleText.text = "ALL SYNERGIES";
        titleText.fontSize = 34;
        titleText.fontStyle = FontStyles.Bold;
        titleText.color = Color.black;
        titleText.alignment = TextAlignmentOptions.Center;
    }

    private void CreateCloseButton(Transform parent)
    {
        GameObject closeObj = new GameObject("CloseButton");
        closeObj.transform.SetParent(parent, false);

        RectTransform closeRect = closeObj.AddComponent<RectTransform>();
        closeRect.anchorMin = new Vector2(1f, 1f);
        closeRect.anchorMax = new Vector2(1f, 1f);
        closeRect.pivot = new Vector2(1f, 1f);
        closeRect.sizeDelta = new Vector2(40f, 40f);
        closeRect.anchoredPosition = new Vector2(-5f, -5f);

        Image closeBg = closeObj.AddComponent<Image>();
        closeBg.color = new Color(0.6f, 0.2f, 0.2f, 1f);

        Button closeButton = closeObj.AddComponent<Button>();
        closeButton.targetGraphic = closeBg;
        closeButton.onClick.AddListener(Hide);

        ColorBlock colors = closeButton.colors;
        colors.normalColor = new Color(0.6f, 0.2f, 0.2f, 1f);
        colors.highlightedColor = new Color(0.8f, 0.3f, 0.3f, 1f);
        colors.pressedColor = new Color(0.5f, 0.15f, 0.15f, 1f);
        closeButton.colors = colors;

        // X text
        GameObject xObj = new GameObject("X");
        xObj.transform.SetParent(closeObj.transform, false);

        RectTransform xRect = xObj.AddComponent<RectTransform>();
        xRect.anchorMin = Vector2.zero;
        xRect.anchorMax = Vector2.one;
        xRect.sizeDelta = Vector2.zero;

        TextMeshProUGUI xText = xObj.AddComponent<TextMeshProUGUI>();
        xText.text = "X";
        xText.fontSize = 24;
        xText.fontStyle = FontStyles.Bold;
        xText.color = Color.white;
        xText.alignment = TextAlignmentOptions.Center;
    }

    private void CreateSynergyList(Transform parent)
    {
        GameObject contentObj = new GameObject("Content");
        contentObj.transform.SetParent(parent, false);

        RectTransform contentRect = contentObj.AddComponent<RectTransform>();
        contentRect.anchorMin = new Vector2(0f, 0f);
        contentRect.anchorMax = new Vector2(1f, 1f);
        contentRect.sizeDelta = Vector2.zero;
        contentRect.offsetMin = new Vector2(20f, 20f);
        contentRect.offsetMax = new Vector2(-20f, -55f);

        float rowHeight = 32f;
        float currentY = 0f;

        // Ability synergies (workhorse-specific abilities)
        if (SynergySystem.Instance.AbilitySynergies.Count > 0)
        {
            CreateSectionHeader(contentObj.transform, "WORKHORSE ABILITIES", ref currentY);
            foreach (var synergy in SynergySystem.Instance.AbilitySynergies)
            {
                CreateRow(contentObj.transform, synergy, rowHeight, ref currentY);
            }
            currentY -= 15f;
        }

        // Global synergies
        if (SynergySystem.Instance.GlobalSynergies.Count > 0)
        {
            CreateSectionHeader(contentObj.transform, "GLOBAL SYNERGIES", ref currentY);
            foreach (var synergy in SynergySystem.Instance.GlobalSynergies)
            {
                CreateRow(contentObj.transform, synergy, rowHeight, ref currentY);
            }
            currentY -= 15f;
        }

        // Adjacent synergies
        if (SynergySystem.Instance.AdjacentSynergies.Count > 0)
        {
            CreateSectionHeader(contentObj.transform, "ADJACENT SYNERGIES", ref currentY);
            foreach (var synergy in SynergySystem.Instance.AdjacentSynergies)
            {
                CreateRow(contentObj.transform, synergy, rowHeight, ref currentY);
            }
            currentY -= 15f;
        }

        // Position synergies
        if (SynergySystem.Instance.PositionSynergies.Count > 0)
        {
            CreateSectionHeader(contentObj.transform, "POSITION SYNERGIES", ref currentY);
            foreach (var synergy in SynergySystem.Instance.PositionSynergies)
            {
                CreateRow(contentObj.transform, synergy, rowHeight, ref currentY);
            }
        }
    }

    private void CreateSectionHeader(Transform parent, string text, ref float currentY)
    {
        GameObject headerObj = new GameObject($"Header_{text}");
        headerObj.transform.SetParent(parent, false);

        RectTransform headerRect = headerObj.AddComponent<RectTransform>();
        headerRect.anchorMin = new Vector2(0f, 1f);
        headerRect.anchorMax = new Vector2(1f, 1f);
        headerRect.pivot = new Vector2(0f, 1f);
        headerRect.sizeDelta = new Vector2(0f, 24f);
        headerRect.anchoredPosition = new Vector2(0f, currentY);

        TextMeshProUGUI headerText = headerObj.AddComponent<TextMeshProUGUI>();
        headerText.text = text;
        headerText.fontSize = 20;
        headerText.fontStyle = FontStyles.Bold | FontStyles.Italic;
        headerText.color = Color.black;
        headerText.alignment = TextAlignmentOptions.Left;

        currentY -= 28f;
    }

    private void CreateRow(Transform parent, Synergy synergy, float height, ref float currentY)
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
        bulletRect.sizeDelta = new Vector2(16f, 16f);
        bulletRect.anchoredPosition = new Vector2(10f, 0f);

        Image bulletImage = bulletObj.AddComponent<Image>();
        bulletImage.color = InactiveBullet;

        // Name + bonus
        GameObject nameObj = new GameObject("Name");
        nameObj.transform.SetParent(rowObj.transform, false);

        RectTransform nameRect = nameObj.AddComponent<RectTransform>();
        nameRect.anchorMin = new Vector2(0f, 0f);
        nameRect.anchorMax = new Vector2(0.6f, 1f);
        nameRect.sizeDelta = Vector2.zero;
        nameRect.offsetMin = new Vector2(30f, 0f);
        nameRect.offsetMax = Vector2.zero;

        TextMeshProUGUI nameText = nameObj.AddComponent<TextMeshProUGUI>();
        // For ability synergies, show just the name; for others show name + bonus
        if (synergy.Type == SynergyType.WorkhorseAbility)
        {
            nameText.text = synergy.Name;
        }
        else
        {
            nameText.text = $"{synergy.Name} (+{synergy.BonusPercent:F0}%)";
        }
        nameText.fontSize = 22;
        nameText.fontStyle = FontStyles.Bold;
        nameText.color = InactiveColor;
        nameText.alignment = TextAlignmentOptions.Left;

        // Description
        GameObject descObj = new GameObject("Description");
        descObj.transform.SetParent(rowObj.transform, false);

        RectTransform descRect = descObj.AddComponent<RectTransform>();
        descRect.anchorMin = new Vector2(0.6f, 0f);
        descRect.anchorMax = new Vector2(1f, 1f);
        descRect.sizeDelta = Vector2.zero;
        descRect.offsetMin = new Vector2(5f, 0f);
        descRect.offsetMax = Vector2.zero;

        TextMeshProUGUI descText = descObj.AddComponent<TextMeshProUGUI>();
        descText.text = synergy.Description;
        descText.fontSize = 18;
        descText.fontStyle = FontStyles.Italic;
        descText.color = Color.black;
        descText.alignment = TextAlignmentOptions.Left;

        var row = new SynergyRow
        {
            Synergy = synergy,
            BulletImage = bulletImage,
            NameText = nameText,
            DescText = descText
        };
        _rows.Add(row);

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
        public TextMeshProUGUI DescText;
    }
}
