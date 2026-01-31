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

    private static readonly Color NormalColor = new Color(0.5f, 0.35f, 0.2f, 1f);
    private static readonly Color ActiveColor = new Color(0.9f, 0.5f, 0.2f, 1f);

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
        _button = UiUtils.CreateTextButton(root, "CHECK", NormalColor, HandleClick, fontSize: 20);
        _background = _button.targetGraphic as Image;
        _text = _button.GetComponentInChildren<TextMeshProUGUI>();
    }

    private void HandleClick()
    {
        _onClick?.Invoke();
    }

    public void UpdateVisual(bool isActive)
    {
        _text.text = isActive ? "EXIT CHECK" : "CHECK";
        UpdateButtonColors(isActive);
    }

    private void UpdateButtonColors(bool isActive)
    {
        Color baseColor = isActive ? ActiveColor : NormalColor;
        _background.color = baseColor;
        _button.colors = UiUtils.CreateColorBlock(baseColor);
    }
}
