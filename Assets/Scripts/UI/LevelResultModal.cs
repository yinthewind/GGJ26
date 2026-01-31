using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelResultModal : MonoBehaviour
{
    private GameObject _overlay;
    private GameObject _panel;
    private TextMeshProUGUI _titleText;
    private TextMeshProUGUI _levelNameText;
    private TextMeshProUGUI _goalInfoText;
    private TextMeshProUGUI _turnsInfoText;
    private TextMeshProUGUI _rewardText;
    private GameObject _rewardRow;
    private GameObject _primaryButton;
    private GameObject _secondaryButton;
    private Button _primaryButtonComponent;
    private Button _secondaryButtonComponent;
    private TextMeshProUGUI _primaryButtonText;
    private TextMeshProUGUI _secondaryButtonText;

    private static readonly Color WinTitleColor = new Color(0.2f, 0.9f, 0.2f);
    private static readonly Color FailTitleColor = new Color(0.9f, 0.2f, 0.2f);
    private static readonly Color GreenButtonColor = new Color(0.2f, 0.7f, 0.2f);
    private static readonly Color BlueButtonColor = new Color(0.2f, 0.4f, 0.7f);

    public bool IsVisible => gameObject.activeSelf;

    public static LevelResultModal Create(Transform parent)
    {
        GameObject obj = new GameObject("LevelResultModal");
        obj.transform.SetParent(parent, false);

        LevelResultModal component = obj.AddComponent<LevelResultModal>();
        component.BuildUI(obj);
        component.Hide();

        return component;
    }

    public void ShowWin()
    {
        gameObject.SetActive(true);
        ModalManager.Instance?.NotifyModalStateChanged(true);
        UpdateForWin();
    }

    public void ShowFail()
    {
        gameObject.SetActive(true);
        ModalManager.Instance?.NotifyModalStateChanged(true);
        UpdateForFail();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        ModalManager.Instance?.NotifyModalStateChanged(false);
    }

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
    }

    private void CreatePanel(Transform parent)
    {
        _panel = new GameObject("Panel");
        _panel.transform.SetParent(parent, false);

        RectTransform panelRect = _panel.AddComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.5f, 0.5f);
        panelRect.anchorMax = new Vector2(0.5f, 0.5f);
        panelRect.pivot = new Vector2(0.5f, 0.5f);
        panelRect.sizeDelta = new Vector2(400f, 280f);
        panelRect.anchoredPosition = new Vector2(0f, 250f);

        Image panelBg = _panel.AddComponent<Image>();
        panelBg.color = new Color(0.12f, 0.12f, 0.18f, 0.98f);

        CreateTitle(_panel.transform);
        CreateContent(_panel.transform);
        CreateButtons(_panel.transform);
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
        titleRect.anchoredPosition = new Vector2(0f, -10f);

        _titleText = titleObj.AddComponent<TextMeshProUGUI>();
        _titleText.text = "LEVEL COMPLETE!";
        _titleText.fontSize = 32;
        _titleText.fontStyle = FontStyles.Bold;
        _titleText.color = WinTitleColor;
        _titleText.alignment = TextAlignmentOptions.Center;
    }

    private void CreateContent(Transform parent)
    {
        // Content container
        GameObject contentObj = new GameObject("Content");
        contentObj.transform.SetParent(parent, false);

        RectTransform contentRect = contentObj.AddComponent<RectTransform>();
        contentRect.anchorMin = new Vector2(0f, 0f);
        contentRect.anchorMax = new Vector2(1f, 1f);
        contentRect.sizeDelta = Vector2.zero;
        contentRect.offsetMin = new Vector2(30f, 70f);
        contentRect.offsetMax = new Vector2(-30f, -60f);

        float currentY = 0f;

        // Level name
        _levelNameText = CreateInfoRow(contentObj.transform, "Level:", "", ref currentY);

        // Goal info
        _goalInfoText = CreateInfoRow(contentObj.transform, "Goal:", "", ref currentY);

        // Turns used
        _turnsInfoText = CreateInfoRow(contentObj.transform, "Turns:", "", ref currentY);

        // Reward row (only visible on win)
        _rewardRow = new GameObject("RewardRow");
        _rewardRow.transform.SetParent(contentObj.transform, false);

        RectTransform rewardRowRect = _rewardRow.AddComponent<RectTransform>();
        rewardRowRect.anchorMin = new Vector2(0f, 1f);
        rewardRowRect.anchorMax = new Vector2(1f, 1f);
        rewardRowRect.pivot = new Vector2(0f, 1f);
        rewardRowRect.sizeDelta = new Vector2(0f, 28f);
        rewardRowRect.anchoredPosition = new Vector2(0f, currentY);

        _rewardText = CreateLabelValuePair(_rewardRow.transform, "Reward:", "");
    }

    private TextMeshProUGUI CreateInfoRow(Transform parent, string label, string value, ref float currentY)
    {
        GameObject rowObj = new GameObject($"Row_{label}");
        rowObj.transform.SetParent(parent, false);

        RectTransform rowRect = rowObj.AddComponent<RectTransform>();
        rowRect.anchorMin = new Vector2(0f, 1f);
        rowRect.anchorMax = new Vector2(1f, 1f);
        rowRect.pivot = new Vector2(0f, 1f);
        rowRect.sizeDelta = new Vector2(0f, 28f);
        rowRect.anchoredPosition = new Vector2(0f, currentY);

        var valueText = CreateLabelValuePair(rowObj.transform, label, value);

        currentY -= 32f;
        return valueText;
    }

    private TextMeshProUGUI CreateLabelValuePair(Transform parent, string label, string value)
    {
        // Label
        GameObject labelObj = new GameObject("Label");
        labelObj.transform.SetParent(parent, false);

        RectTransform labelRect = labelObj.AddComponent<RectTransform>();
        labelRect.anchorMin = new Vector2(0f, 0f);
        labelRect.anchorMax = new Vector2(0.35f, 1f);
        labelRect.sizeDelta = Vector2.zero;
        labelRect.offsetMin = Vector2.zero;
        labelRect.offsetMax = Vector2.zero;

        TextMeshProUGUI labelText = labelObj.AddComponent<TextMeshProUGUI>();
        labelText.text = label;
        labelText.fontSize = 18;
        labelText.fontStyle = FontStyles.Normal;
        labelText.color = new Color(0.7f, 0.7f, 0.8f);
        labelText.alignment = TextAlignmentOptions.Left;

        // Value
        GameObject valueObj = new GameObject("Value");
        valueObj.transform.SetParent(parent, false);

        RectTransform valueRect = valueObj.AddComponent<RectTransform>();
        valueRect.anchorMin = new Vector2(0.35f, 0f);
        valueRect.anchorMax = new Vector2(1f, 1f);
        valueRect.sizeDelta = Vector2.zero;
        valueRect.offsetMin = Vector2.zero;
        valueRect.offsetMax = Vector2.zero;

        TextMeshProUGUI valueText = valueObj.AddComponent<TextMeshProUGUI>();
        valueText.text = value;
        valueText.fontSize = 18;
        valueText.fontStyle = FontStyles.Bold;
        valueText.color = Color.white;
        valueText.alignment = TextAlignmentOptions.Left;

        return valueText;
    }

    private void CreateButtons(Transform parent)
    {
        // Button container at bottom
        GameObject buttonContainer = new GameObject("ButtonContainer");
        buttonContainer.transform.SetParent(parent, false);

        RectTransform containerRect = buttonContainer.AddComponent<RectTransform>();
        containerRect.anchorMin = new Vector2(0f, 0f);
        containerRect.anchorMax = new Vector2(1f, 0f);
        containerRect.pivot = new Vector2(0.5f, 0f);
        containerRect.sizeDelta = new Vector2(0f, 50f);
        containerRect.anchoredPosition = new Vector2(0f, 15f);

        // Primary button (left side when two buttons, center when one)
        _primaryButton = CreateButton(buttonContainer.transform, "Next Level", GreenButtonColor, OnPrimaryClick);
        RectTransform primaryRect = _primaryButton.GetComponent<RectTransform>();
        primaryRect.anchorMin = new Vector2(0.1f, 0f);
        primaryRect.anchorMax = new Vector2(0.48f, 1f);
        primaryRect.sizeDelta = Vector2.zero;
        primaryRect.offsetMin = Vector2.zero;
        primaryRect.offsetMax = Vector2.zero;

        _primaryButtonComponent = _primaryButton.GetComponent<Button>();
        _primaryButtonText = _primaryButton.GetComponentInChildren<TextMeshProUGUI>();

        // Secondary button (right side, only visible on win)
        _secondaryButton = CreateButton(buttonContainer.transform, "Replay", BlueButtonColor, OnSecondaryClick);
        RectTransform secondaryRect = _secondaryButton.GetComponent<RectTransform>();
        secondaryRect.anchorMin = new Vector2(0.52f, 0f);
        secondaryRect.anchorMax = new Vector2(0.9f, 1f);
        secondaryRect.sizeDelta = Vector2.zero;
        secondaryRect.offsetMin = Vector2.zero;
        secondaryRect.offsetMax = Vector2.zero;

        _secondaryButtonComponent = _secondaryButton.GetComponent<Button>();
        _secondaryButtonText = _secondaryButton.GetComponentInChildren<TextMeshProUGUI>();
    }

    private GameObject CreateButton(Transform parent, string text, Color color, System.Action callback)
    {
        GameObject buttonObj = new GameObject($"Button_{text}");
        buttonObj.transform.SetParent(parent, false);

        UiUtils.CreateTextButton(buttonObj, text, color, callback, fontSize: 18);

        return buttonObj;
    }

    private void UpdateForWin()
    {
        var config = LevelManager.Instance.CurrentConfig;
        var goal = GoalManager.Instance.CurrentGoal;
        string currentLevelId = LevelManager.Instance.CurrentLevelId;
        string nextLevelId = LevelDefinitions.GetNextLevelId(currentLevelId);

        _titleText.text = "LEVEL COMPLETE!";
        _titleText.color = WinTitleColor;

        _levelNameText.text = config?.LevelName ?? "Unknown";
        _goalInfoText.text = $"{goal?.Name ?? "Unknown"} - Completed!";
        _turnsInfoText.text = $"{TurnManager.Instance.CurrentTurn - 1} / {config?.TurnLimit ?? 0}";

        // Show reward
        _rewardRow.SetActive(true);
        _rewardText.text = $"+${config?.ProjectReward ?? 0}";
        _rewardText.color = new Color(1f, 0.9f, 0.3f);

        // Show both buttons if there's a next level
        if (nextLevelId != null)
        {
            _primaryButtonText.text = "Next Level";
            _secondaryButton.SetActive(true);
            _secondaryButtonText.text = "Replay";

            // Reposition primary button to left side
            RectTransform primaryRect = _primaryButton.GetComponent<RectTransform>();
            primaryRect.anchorMin = new Vector2(0.1f, 0f);
            primaryRect.anchorMax = new Vector2(0.48f, 1f);
        }
        else
        {
            // Last level - only show replay
            _primaryButtonText.text = "Replay";
            _secondaryButton.SetActive(false);

            // Center the primary button
            RectTransform primaryRect = _primaryButton.GetComponent<RectTransform>();
            primaryRect.anchorMin = new Vector2(0.25f, 0f);
            primaryRect.anchorMax = new Vector2(0.75f, 1f);
        }
    }

    private void UpdateForFail()
    {
        var config = LevelManager.Instance.CurrentConfig;
        var goal = GoalManager.Instance.CurrentGoal;
        float progress = GoalManager.Instance.GetGoalProgress();

        _titleText.text = "LEVEL FAILED";
        _titleText.color = FailTitleColor;

        _levelNameText.text = config?.LevelName ?? "Unknown";
        _goalInfoText.text = $"{goal?.Name ?? "Unknown"} - {progress * 100:F0}%";
        _turnsInfoText.text = $"{config?.TurnLimit ?? 0} / {config?.TurnLimit ?? 0} (out of turns)";

        // Show consolation reward (30%)
        int consolationReward = Mathf.RoundToInt((config?.ProjectReward ?? 0) * 0.3f);
        _rewardRow.SetActive(true);
        _rewardText.text = $"+${consolationReward} (30%)";
        _rewardText.color = new Color(0.8f, 0.7f, 0.5f); // Dimmer gold for consolation

        // Show both buttons if there's a next level
        string currentLevelId = LevelManager.Instance.CurrentLevelId;
        string nextLevelId = LevelDefinitions.GetNextLevelId(currentLevelId);

        if (nextLevelId != null)
        {
            _primaryButtonText.text = "Next Level";
            _secondaryButton.SetActive(true);
            _secondaryButtonText.text = "Retry";

            // Reposition primary button to left side
            RectTransform primaryRect = _primaryButton.GetComponent<RectTransform>();
            primaryRect.anchorMin = new Vector2(0.1f, 0f);
            primaryRect.anchorMax = new Vector2(0.48f, 1f);
        }
        else
        {
            // Last level - only show retry
            _primaryButtonText.text = "Retry";
            _secondaryButton.SetActive(false);

            // Center the primary button
            RectTransform primaryRect = _primaryButton.GetComponent<RectTransform>();
            primaryRect.anchorMin = new Vector2(0.25f, 0f);
            primaryRect.anchorMax = new Vector2(0.75f, 1f);
        }
    }

    private void OnPrimaryClick()
    {
        string currentLevelId = LevelManager.Instance.CurrentLevelId;
        string nextLevelId = LevelDefinitions.GetNextLevelId(currentLevelId);

        if (nextLevelId != null)
        {
            // Award consolation reward only when advancing after fail (not retry)
            if (LevelManager.Instance.IsLevelFailed)
            {
                var config = LevelManager.Instance.CurrentConfig;
                int consolationReward = Mathf.RoundToInt((config?.ProjectReward ?? 0) * 0.3f);
                if (consolationReward > 0)
                {
                    PlayerProgress.Instance.AddDollar(consolationReward);
                }
            }

            // Go to next level (whether won or failed with consolation)
            LevelManager.Instance.LoadLevel(nextLevelId);
        }
        else
        {
            // Last level - restart/replay
            LevelManager.Instance.RestartLevel();
        }

        Hide();
    }

    private void OnSecondaryClick()
    {
        // Secondary button is always "Replay" on win
        LevelManager.Instance.RestartLevel();
        Hide();
    }
}
