using UnityEngine;
using UnityEngine.UI;

public class ShowSynergiesButton : MonoBehaviour
{
    private Button _button;
    private SynergyModal _modal;

    private static readonly string ButtonSpritePath = "Sprites/UIUX/performance_button";

    public static ShowSynergiesButton Create(Transform parent, float width, float height)
    {
        GameObject obj = new GameObject("ShowSynergiesButton");
        obj.transform.SetParent(parent, false);

        ShowSynergiesButton component = obj.AddComponent<ShowSynergiesButton>();
        component.BuildUI(obj);

        return component;
    }

    public void BindModal(SynergyModal modal)
    {
        _modal = modal;
    }

    private void BuildUI(GameObject root)
    {
        Sprite sprite = SpriteLoader.Instance.GetSprite(ButtonSpritePath);
        _button = UiUtils.CreateSpriteButton(root, "协同效果", sprite, HandleClick, fontSize: 18);
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
