using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShowSynergiesButton : MonoBehaviour
{
    private Button _button;
    private TextMeshProUGUI _text;
    private Image _background;
    private SynergyModal _modal;

    public static ShowSynergiesButton Create(Transform parent, float width, float height)
    {
        GameObject obj = new GameObject("ShowSynergiesButton");
        obj.transform.SetParent(parent, false);

        ShowSynergiesButton component = obj.AddComponent<ShowSynergiesButton>();
        component.BuildUI(obj, width, height);

        return component;
    }

    public void BindModal(SynergyModal modal)
    {
        _modal = modal;
    }

    private void BuildUI(GameObject root, float width, float height)
    {
        // RectTransform for sizing (positioning handled by parent container)
        RectTransform rect = root.AddComponent<RectTransform>();

        _background = root.AddComponent<Image>();
        _background.color = new Color(0.3f, 0.4f, 0.6f, 1f);

        _button = root.AddComponent<Button>();
        _button.targetGraphic = _background;
        _button.onClick.AddListener(HandleClick);

        ColorBlock colors = _button.colors;
        colors.normalColor = new Color(0.3f, 0.4f, 0.6f, 1f);
        colors.highlightedColor = new Color(0.4f, 0.5f, 0.7f, 1f);
        colors.pressedColor = new Color(0.25f, 0.35f, 0.5f, 1f);
        colors.disabledColor = new Color(0.3f, 0.3f, 0.3f, 0.5f);
        _button.colors = colors;

        CreateButtonText(root.transform);
    }

    private void CreateButtonText(Transform parent)
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
        _text.text = "SYNERGIES";
        _text.fontSize = 18;
        _text.fontStyle = FontStyles.Bold;
        _text.color = Color.white;
        _text.alignment = TextAlignmentOptions.Center;
    }

    private void HandleClick()
    {
        if (_modal != null)
        {
            if (_modal.IsVisible)
            {
                _modal.Hide();
            }
            else
            {
                _modal.Show();
            }
        }
    }
}
