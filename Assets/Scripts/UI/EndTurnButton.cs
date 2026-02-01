using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class EndTurnButton : MonoBehaviour
{
    public UnityEvent OnClicked { get; private set; } = new UnityEvent();

    private Button _button;
    private Image _image;
    private Action _onClick;

    public static EndTurnButton Create(Transform parent, Action onClick)
    {
        GameObject obj = new GameObject("EndTurnButton");
        obj.transform.SetParent(parent, false);

        EndTurnButton component = obj.AddComponent<EndTurnButton>();
        component._onClick = onClick;
        component.BuildUI(obj);

        return component;
    }

    private void BuildUI(GameObject root)
    {
        RectTransform rect = root.AddComponent<RectTransform>();

        // Add Image component with EndTurn sprite
        _image = root.AddComponent<Image>();
        _image.sprite = SpriteLoader.Instance.GetSprite("Sprites/UIUX/EndTurn");
        _image.SetNativeSize();

        // Add Button component
        _button = root.AddComponent<Button>();
        _button.targetGraphic = _image;
        _button.onClick.AddListener(HandleClick);

        // Configure button colors for hover/pressed states
        ColorBlock colors = _button.colors;
        colors.normalColor = Color.white;
        colors.highlightedColor = new Color(0.9f, 0.9f, 0.9f, 1f);
        colors.pressedColor = new Color(0.7f, 0.7f, 0.7f, 1f);
        colors.disabledColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        _button.colors = colors;
    }

    private void HandleClick()
    {
        _onClick?.Invoke();
        OnClicked?.Invoke();
    }

    public void SetInteractable(bool interactable)
    {
        _button.interactable = interactable;
    }
}
