using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class RestartLevelButton : MonoBehaviour
{
    public UnityEvent OnClicked { get; private set; } = new UnityEvent();

    private Button _button;
    private Action _onClick;

    private static readonly Color ButtonColor = new Color(0.6f, 0.3f, 0.2f, 1f);

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
        _button = UiUtils.CreateTextButton(root, "RESTART", ButtonColor, HandleClick, fontSize: 20);
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
