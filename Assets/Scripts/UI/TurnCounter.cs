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
        UpdateDisplay();
    }

    private void OnDestroy()
    {
        TurnManager.Instance.OnTurnStarted -= UpdateDisplay;
    }

    private void BuildUI(GameObject root, float width, float height)
    {
        RectTransform rect = root.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(0f, 1f);
        rect.pivot = new Vector2(0f, 1f);
        rect.sizeDelta = new Vector2(width, height);
        rect.anchoredPosition = new Vector2(10f, -10f);

        _background = root.AddComponent<Image>();
        _background.color = new Color(0.1f, 0.1f, 0.15f, 0.9f);

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
        _turnText.text = "Turn 1";
        _turnText.fontSize = 22;
        _turnText.fontStyle = FontStyles.Bold;
        _turnText.color = Color.white;
        _turnText.alignment = TextAlignmentOptions.Center;
    }

    private void UpdateDisplay()
    {
        _turnText.text = $"Turn {TurnManager.Instance.CurrentTurn}";
    }
}
