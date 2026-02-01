using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CheckButton : MonoBehaviour
{
    private Button _button;
    private TextMeshProUGUI _text;
    private Image _background;
    private Action _onClick;

    private static readonly string ButtonSpritePath = "Sprites/UIUX/performance_button";
    private static readonly Color NormalTint = Color.white;
    private static readonly Color ActiveTint = new Color(1f, 0.7f, 0.5f, 1f);

    public static CheckButton Create(Transform parent, float width, float height, Action onClick)
    {
        GameObject obj = new GameObject("CheckButton");
        obj.transform.SetParent(parent, false);

        CheckButton component = obj.AddComponent<CheckButton>();
        component._onClick = onClick;
        component.BuildUI(obj);

        return component;
    }

    private void BuildUI(GameObject root)
    {
        Sprite sprite = SpriteLoader.Instance.GetSprite(ButtonSpritePath);
        _button = UiUtils.CreateSpriteButton(root, "检查", sprite, HandleClick, fontSize: 20);
        _background = _button.targetGraphic as Image;
        _text = _button.GetComponentInChildren<TextMeshProUGUI>();
    }

    private void HandleClick()
    {
        _onClick?.Invoke();
    }

    public void UpdateVisual(bool isActive)
    {
        _text.text = isActive ? "退出检查" : "检查";
        UpdateButtonTint(isActive);
    }

    private void UpdateButtonTint(bool isActive)
    {
        Color tint = isActive ? ActiveTint : NormalTint;
        _background.color = tint;
    }
}
