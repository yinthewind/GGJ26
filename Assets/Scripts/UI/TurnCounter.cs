using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TurnCounter : MonoBehaviour
{
    private TextMeshProUGUI _turnText;
    private Image _background;

    public static TurnCounter Create(Transform parent, float width, float height)
    {
        GameObject obj = new GameObject("TurnCounter");
        obj.transform.SetParent(parent, false);

        TurnCounter component = obj.AddComponent<TurnCounter>();
        component.BuildUI(obj, width, height);

        return component;
    }

    private void Start()
    {
        TurnManager.Instance.OnTurnStarted += UpdateDisplay;
        LevelManager.Instance.OnLevelLoaded += HandleLevelLoaded;
        UpdateDisplay();
    }

    private void OnDestroy()
    {
        TurnManager.Instance.OnTurnStarted -= UpdateDisplay;
        LevelManager.Instance.OnLevelLoaded -= HandleLevelLoaded;
    }

    private void HandleLevelLoaded(string levelId)
    {
        UpdateDisplay();
    }

    private void BuildUI(GameObject root, float width, float height)
    {
        RectTransform rect = root.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(0f, 1f);
        rect.pivot = new Vector2(0f, 1f);
        rect.anchoredPosition = new Vector2(10f, -10f);

        _background = root.AddComponent<Image>();
        _background.sprite = SpriteLoader.Instance.GetSprite("Sprites/UIUX/day");
        _background.type = Image.Type.Sliced;

        // Use the sprite's native size if available
        if (_background.sprite != null)
        {
            _background.SetNativeSize();
            width = rect.sizeDelta.x;
            height = rect.sizeDelta.y;
        }
        else
        {
            rect.sizeDelta = new Vector2(width, height);
        }

        CreateTurnText(root.transform, width, height);
    }

    private void CreateTurnText(Transform parent, float width, float height)
    {
        GameObject textObj = new GameObject("TurnText");
        textObj.transform.SetParent(parent, false);

        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;
        textRect.offsetMin = new Vector2(10f, 5f);
        textRect.offsetMax = new Vector2(-10f, -5f);

        _turnText = textObj.AddComponent<TextMeshProUGUI>();
        _turnText.text = "Lv.1 - Turn 1/10";
        _turnText.fontSize = 22;
        _turnText.fontStyle = FontStyles.Bold;
        _turnText.color = Color.white;
        _turnText.alignment = TextAlignmentOptions.Center;
    }

    private void UpdateDisplay()
    {
        int currentTurn = TurnManager.Instance.CurrentTurn;
        int turnLimit = LevelManager.Instance.CurrentConfig?.TurnLimit ?? 0;
        string levelId = LevelManager.Instance.CurrentLevelId ?? "level_1";
        string levelNumber = levelId.Replace("level_", "");
        _turnText.text = $"Lv.{levelNumber} - Turn {currentTurn}/{turnLimit}";
    }
}
