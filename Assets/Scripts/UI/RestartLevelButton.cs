using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class RestartLevelButton : MonoBehaviour
{
    public UnityEvent OnClicked { get; private set; } = new UnityEvent();

    private Button _button;
    private Action _onClick;

    private static readonly string ButtonSpritePath = "Sprites/UIUX/performance_button";

    public static RestartLevelButton Create(Transform parent, float width, float height, Action onClick)
    {
        GameObject obj = new GameObject("RestartLevelButton");
        obj.transform.SetParent(parent, false);

        RestartLevelButton component = obj.AddComponent<RestartLevelButton>();
        component._onClick = onClick;
        component.BuildUI(obj);

        return component;
    }

    private void BuildUI(GameObject root)
    {
        Sprite sprite = SpriteLoader.Instance.GetSprite(ButtonSpritePath);
        _button = UiUtils.CreateSpriteButton(root, "RESTART", sprite, HandleClick, fontSize: 20);
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
