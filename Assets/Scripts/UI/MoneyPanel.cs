using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MoneyPanel : MonoBehaviour
{
    private Image _background;
    private TextMeshProUGUI _dollarText;

    public static MoneyPanel Create(Transform parent, float width, float height)
    {
        GameObject obj = new GameObject("MoneyPanel");
        obj.transform.SetParent(parent, false);

        MoneyPanel component = obj.AddComponent<MoneyPanel>();
        component.BuildUI(obj, width, height);

        return component;
    }

    private void Start()
    {
        PlayerProgress.Instance.OnDollarChanged += HandleDollarChanged;
        UpdateDisplay();
    }

    private void OnDestroy()
    {
        if (PlayerProgress.Instance != null)
        {
            PlayerProgress.Instance.OnDollarChanged -= HandleDollarChanged;
        }
    }

    private void BuildUI(GameObject root, float width, float height)
    {
        RectTransform rect = root.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0f, 1f);
        rect.anchorMax = new Vector2(0f, 1f);
        rect.pivot = new Vector2(0f, 1f);
        rect.sizeDelta = new Vector2(width, height);
        rect.anchoredPosition = new Vector2(10f, -60f); // Below TurnCounter

        // Background using money sprite
        _background = root.AddComponent<Image>();
        Sprite moneySprite = SpriteLoader.Instance.GetSprite("Sprites/UIUX/money");
        if (moneySprite != null)
        {
            _background.sprite = moneySprite;
            _background.type = Image.Type.Sliced;
        }
        else
        {
            _background.color = new Color(0.2f, 0.5f, 0.2f, 0.9f);
        }

        CreateDollarText(root.transform, width, height);
    }

    private void CreateDollarText(Transform parent, float width, float height)
    {
        GameObject textObj = new GameObject("DollarText");
        textObj.transform.SetParent(parent, false);

        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;
        textRect.offsetMin = new Vector2(5f, 2f);
        textRect.offsetMax = new Vector2(-5f, -2f);

        _dollarText = textObj.AddComponent<TextMeshProUGUI>();
        _dollarText.text = "0";
        _dollarText.fontSize = 20;
        _dollarText.fontStyle = FontStyles.Bold;
        _dollarText.color = Color.white;
        _dollarText.alignment = TextAlignmentOptions.Center;
    }

    private void HandleDollarChanged(int newDollar)
    {
        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        _dollarText.text = $"{PlayerProgress.Instance.CurrentDollar}";
    }
}
