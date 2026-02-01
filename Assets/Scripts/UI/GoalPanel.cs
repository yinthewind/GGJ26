using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GoalPanel : MonoBehaviour
{
    private TextMeshProUGUI _goalNameText;
    private TextMeshProUGUI _goalDescText;
    private TextMeshProUGUI _progressText;
    private Image _background;
    private Image _progressBar;
    private Image _progressBarBackground;

    public static GoalPanel Create(Transform parent, float width, float height)
    {
        GameObject obj = new GameObject("GoalPanel");
        obj.transform.SetParent(parent, false);

        GoalPanel component = obj.AddComponent<GoalPanel>();
        component.BuildUI(obj, width, height);

        return component;
    }

    private void Start()
    {
        GoalManager.Instance.OnGoalCompleted += HandleGoalCompleted;
        GoalManager.Instance.OnGoalChanged += HandleGoalChanged;
        TurnManager.Instance.OnTurnEnded += UpdateProgress;

        UpdateDisplay();
    }

    private void OnDestroy()
    {
        GoalManager.Instance.OnGoalCompleted -= HandleGoalCompleted;
        GoalManager.Instance.OnGoalChanged -= HandleGoalChanged;
        TurnManager.Instance.OnTurnEnded -= UpdateProgress;
    }

    private void BuildUI(GameObject root, float width, float height)
    {
        RectTransform rect = root.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(1f, 1f);
        rect.anchorMax = new Vector2(1f, 1f);
        rect.pivot = new Vector2(1f, 1f);
        rect.sizeDelta = new Vector2(width, height);
        rect.anchoredPosition = new Vector2(-10f, -10f);

        _background = root.AddComponent<Image>();
        _background.color = new Color(0.1f, 0.1f, 0.15f, 0.9f);

        CreateGoalNameText(root.transform, width);
        CreateGoalDescText(root.transform, width);
        CreateProgressBar(root.transform, width, height);
    }

    private void CreateGoalNameText(Transform parent, float width)
    {
        GameObject textObj = new GameObject("GoalNameText");
        textObj.transform.SetParent(parent, false);

        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = new Vector2(0f, 1f);
        textRect.anchorMax = new Vector2(1f, 1f);
        textRect.pivot = new Vector2(0.5f, 1f);
        textRect.sizeDelta = new Vector2(0f, 25f);
        textRect.anchoredPosition = new Vector2(0f, -5f);
        textRect.offsetMin = new Vector2(10f, textRect.offsetMin.y);
        textRect.offsetMax = new Vector2(-10f, textRect.offsetMax.y);

        _goalNameText = textObj.AddComponent<TextMeshProUGUI>();
        _goalNameText.text = "目标";
        _goalNameText.fontSize = 16;
        _goalNameText.font = UiUtils.GetChineseFont();
        _goalNameText.fontStyle = FontStyles.Bold;
        _goalNameText.color = new Color(1f, 0.85f, 0.3f);
        _goalNameText.alignment = TextAlignmentOptions.Left;
    }

    private void CreateGoalDescText(Transform parent, float width)
    {
        GameObject textObj = new GameObject("GoalDescText");
        textObj.transform.SetParent(parent, false);

        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = new Vector2(0f, 1f);
        textRect.anchorMax = new Vector2(1f, 1f);
        textRect.pivot = new Vector2(0.5f, 1f);
        textRect.sizeDelta = new Vector2(0f, 20f);
        textRect.anchoredPosition = new Vector2(0f, -30f);
        textRect.offsetMin = new Vector2(10f, textRect.offsetMin.y);
        textRect.offsetMax = new Vector2(-10f, textRect.offsetMax.y);

        _goalDescText = textObj.AddComponent<TextMeshProUGUI>();
        _goalDescText.text = "描述";
        _goalDescText.fontSize = 12;
        _goalDescText.font = UiUtils.GetChineseFont();
        _goalDescText.color = new Color(0.8f, 0.8f, 0.8f);
        _goalDescText.alignment = TextAlignmentOptions.Left;
    }

    private void CreateProgressBar(Transform parent, float width, float height)
    {
        // Progress bar background
        GameObject bgObj = new GameObject("ProgressBarBg");
        bgObj.transform.SetParent(parent, false);

        RectTransform bgRect = bgObj.AddComponent<RectTransform>();
        bgRect.anchorMin = new Vector2(0f, 0f);
        bgRect.anchorMax = new Vector2(1f, 0f);
        bgRect.pivot = new Vector2(0.5f, 0f);
        bgRect.sizeDelta = new Vector2(-20f, 15f);
        bgRect.anchoredPosition = new Vector2(0f, 10f);

        _progressBarBackground = bgObj.AddComponent<Image>();
        _progressBarBackground.color = new Color(0.2f, 0.2f, 0.25f, 1f);

        // Progress bar fill
        GameObject fillObj = new GameObject("ProgressBarFill");
        fillObj.transform.SetParent(bgObj.transform, false);

        RectTransform fillRect = fillObj.AddComponent<RectTransform>();
        fillRect.anchorMin = Vector2.zero;
        fillRect.anchorMax = new Vector2(0f, 1f);
        fillRect.pivot = new Vector2(0f, 0.5f);
        fillRect.sizeDelta = new Vector2(0f, 0f);
        fillRect.anchoredPosition = Vector2.zero;

        _progressBar = fillObj.AddComponent<Image>();
        _progressBar.color = new Color(0.3f, 0.7f, 0.4f, 1f);

        // Progress text
        GameObject textObj = new GameObject("ProgressText");
        textObj.transform.SetParent(bgObj.transform, false);

        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;
        textRect.anchoredPosition = Vector2.zero;

        _progressText = textObj.AddComponent<TextMeshProUGUI>();
        _progressText.text = "0%";
        _progressText.fontSize = 10;
        _progressText.color = Color.white;
        _progressText.alignment = TextAlignmentOptions.Center;
    }

    private void HandleGoalCompleted(GoalDefinition goal)
    {
        UpdateDisplay();
    }

    private void HandleGoalChanged(GoalDefinition goal)
    {
        UpdateDisplay();
    }

    private void UpdateProgress()
    {
        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        var goal = GoalManager.Instance.CurrentGoal;

        if (goal == null)
        {
            _goalNameText.text = "所有目标已完成!";
            _goalDescText.text = "恭喜!";
            SetProgress(1f);
            return;
        }

        _goalNameText.text = goal.Name;
        _goalDescText.text = goal.Description;

        float progress = GoalManager.Instance.GetGoalProgress();
        SetProgress(progress);
    }

    private void SetProgress(float progress)
    {
        progress = Mathf.Clamp01(progress);

        RectTransform fillRect = _progressBar.GetComponent<RectTransform>();
        RectTransform bgRect = _progressBarBackground.GetComponent<RectTransform>();

        float width = bgRect.rect.width * progress;
        fillRect.anchorMax = new Vector2(progress, 1f);

        _progressText.text = $"{(progress * 100):F0}%";
    }
}
