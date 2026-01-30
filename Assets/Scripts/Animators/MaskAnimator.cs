using UnityEngine;
using TMPro;

public class MaskAnimator : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    private TextMeshPro _questionText;

    public bool IsVisible => _spriteRenderer != null && _spriteRenderer.enabled;

    public static MaskAnimator Create(Transform parent)
    {
        GameObject obj = new GameObject("Mask");
        obj.transform.SetParent(parent, false);
        obj.transform.localPosition = new Vector3(0f, 0.5f, 0f);

        MaskAnimator mask = obj.AddComponent<MaskAnimator>();
        mask.BuildVisual(obj);

        return mask;
    }

    private void BuildVisual(GameObject root)
    {
        _spriteRenderer = root.AddComponent<SpriteRenderer>();

        // Create 1x1 white texture
        Texture2D texture = new Texture2D(1, 1);
        texture.SetPixel(0, 0, Color.white);
        texture.Apply();

        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1f);
        _spriteRenderer.sprite = sprite;

        // Darker gray color
        _spriteRenderer.color = new Color(0.2f, 0.2f, 0.25f, 1f);

        // Above workhorse sprites
        _spriteRenderer.sortingOrder = 10;

        // Scale down
        root.transform.localScale = new Vector3(0.8f, 0.8f, 1f);

        // Set default visible
        _spriteRenderer.enabled = true;

        // Create ??? text
        CreateQuestionText(root.transform);
    }

    private void CreateQuestionText(Transform parent)
    {
        GameObject textObj = new GameObject("QuestionText");
        textObj.transform.SetParent(parent, false);
        textObj.transform.localPosition = new Vector3(0f, 0f, -0.1f);  // Slightly in front

        _questionText = textObj.AddComponent<TextMeshPro>();
        _questionText.text = "???";
        _questionText.fontSize = 4f;
        _questionText.fontStyle = FontStyles.Bold;
        _questionText.color = new Color(0.6f, 0.6f, 0.6f, 1f);
        _questionText.alignment = TextAlignmentOptions.Center;
        _questionText.sortingOrder = 11;  // Above the mask sprite

        // Size the text rect
        RectTransform textRect = _questionText.rectTransform;
        textRect.sizeDelta = new Vector2(2f, 1f);
    }

    public void SetVisible(bool visible)
    {
        if (_spriteRenderer != null)
        {
            _spriteRenderer.enabled = visible;
        }
        if (_questionText != null)
        {
            _questionText.enabled = visible;
        }
    }
}
