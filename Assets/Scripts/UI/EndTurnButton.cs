using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class EndTurnButton : MonoBehaviour
{
    public UnityEvent OnClicked { get; private set; } = new UnityEvent();

    private Button _button;
    private TextMeshProUGUI _text;
    private Image _background;

    public static EndTurnButton Create(Transform parent, float width, float height)
    {
        GameObject obj = new GameObject("EndTurnButton");
        obj.transform.SetParent(parent, false);

        EndTurnButton component = obj.AddComponent<EndTurnButton>();
        component.BuildUI(obj, width, height);

        return component;
    }

    private void BuildUI(GameObject root, float width, float height)
    {
        // RectTransform for sizing (positioning handled by parent container)
        RectTransform rect = root.AddComponent<RectTransform>();

        _background = root.AddComponent<Image>();
        _background.color = new Color(0.2f, 0.6f, 0.3f, 1f);

        _button = root.AddComponent<Button>();
        _button.targetGraphic = _background;
        _button.onClick.AddListener(HandleClick);

        ColorBlock colors = _button.colors;
        colors.normalColor = new Color(0.2f, 0.6f, 0.3f, 1f);
        colors.highlightedColor = new Color(0.3f, 0.7f, 0.4f, 1f);
        colors.pressedColor = new Color(0.15f, 0.5f, 0.25f, 1f);
        colors.disabledColor = new Color(0.3f, 0.3f, 0.3f, 0.5f);
        _button.colors = colors;

        CreateButtonText(root.transform, width, height);
    }

    private void CreateButtonText(Transform parent, float width, float height)
    {
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(parent, false);

        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        _text = textObj.AddComponent<TextMeshProUGUI>();
        _text.text = "END TURN";
        _text.fontSize = 24;
        _text.fontStyle = FontStyles.Bold;
        _text.color = Color.white;
        _text.alignment = TextAlignmentOptions.Center;
    }

    private void HandleClick()
    {
        TurnManager.Instance.EndTurn();
        OnClicked?.Invoke();
    }

    public void SetInteractable(bool interactable)
    {
        _button.interactable = interactable;
    }
}
