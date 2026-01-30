# Claude Code Guidelines for GGJ26

## Unity Project Rules

- Do NOT create `*.meta` files. Unity generates these automatically when it detects new files.

## UI Implementation Guidelines

### Architecture
- **Programmatic UI** - Build UI entirely in code, no prefabs
- **Event-driven** - Use `UnityEvent<T>` for component communication
- **Factory pattern** - Each UI component should have a static `Create()` method

### Component Structure
```csharp
public class MyComponent : MonoBehaviour
{
    public UnityEvent<bool> OnStateChanged { get; private set; } = new UnityEvent<bool>();

    public static MyComponent Create(Transform parent, float width, float height, ...)
    {
        GameObject obj = new GameObject("MyComponent");
        obj.transform.SetParent(parent, false);

        MyComponent component = obj.AddComponent<MyComponent>();
        component.BuildUI(obj, width, height, ...);

        return component;
    }

    private void BuildUI(GameObject root, float width, float height, ...)
    {
        // Create RectTransform with center anchors + explicit size
        RectTransform rect = root.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(width, height);

        // Build child elements...
    }
}
```

### Key Patterns
- **Parent-controlled sizing** - Parent passes explicit width/height to children
- **Center anchors + sizeDelta** - Avoids Unity's default RectTransform sizing issues
- **Bind...() methods** - For connecting UI to data sources
- **No Inspector dependencies** - All components configured programmatically
