using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Singleton utility class for loading and caching sprites from Resources.
/// Supports individual sprites and multi-sprite texture slices.
/// Sprites are loaded on-demand and cached for future access.
///
/// Multi-sprite textures are handled by parsing the path (e.g., "Test/fishes_8")
/// to extract the base texture ("Test/fishes") and loading all slices.
/// </summary>
public class SpriteLoader
{
    #region Singleton

    private static SpriteLoader _instance;

    /// <summary>
    /// Singleton instance. Automatically preloads all sprites on first access.
    /// </summary>
    public static SpriteLoader Instance
    {
        get
        {
            _instance ??= new SpriteLoader();
            return _instance;
        }
    }

    #endregion

    #region Private Fields

    private readonly Dictionary<string, Sprite> spriteCache = new Dictionary<string, Sprite>();
    private readonly Dictionary<string, Texture2D> textureCache = new Dictionary<string, Texture2D>();

    #endregion

    #region Constructor

    /// <summary>
    /// Private constructor - sprites are loaded on-demand.
    /// </summary>
    private SpriteLoader()
    {
        Debug.Log("SpriteLoader: Initialized");
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Gets a sprite by resource path, loading on-demand and caching for future access.
    /// Supports:
    /// - Single sprites: "Test/background"
    /// - Multi-sprite texture slices: "Test/fishes_0", "Test/fishes_1", etc.
    /// </summary>
    /// <param name="resourcePath">Path relative to Resources folder (without extension)</param>
    /// <returns>The sprite, or null if not found</returns>
    public Sprite GetSprite(string resourcePath)
    {
        if (string.IsNullOrEmpty(resourcePath))
        {
            Debug.LogWarning("SpriteLoader: Resource path is null or empty");
            return null;
        }

        // Return cached sprite if available
        if (spriteCache.TryGetValue(resourcePath, out Sprite cached))
        {
            return cached;
        }

        // Try direct load first (works for single sprites)
        Sprite sprite = Resources.Load<Sprite>(resourcePath);
        if (sprite != null)
        {
            spriteCache[resourcePath] = sprite;
            return sprite;
        }

        // Try loading as multi-sprite texture slice
        sprite = TryLoadMultiSpriteSlice(resourcePath);
        if (sprite != null)
        {
            return sprite;
        }

        Debug.LogWarning($"SpriteLoader: Sprite not found: {resourcePath}");
        return null;
    }


    /// <summary>
    /// Gets a texture by resource path, loading on-demand and caching, then converts to sprite.
    /// </summary>
    /// <param name="resourcePath">Path relative to Resources folder (without extension)</param>
    /// <param name="pixelsPerUnit">Pixels per unit for the sprite (default: 100)</param>
    /// <returns>The sprite created from texture, or null if texture not found</returns>
    public Sprite GetTexture(string resourcePath, float pixelsPerUnit = 100f)
    {
        if (string.IsNullOrEmpty(resourcePath))
        {
            Debug.LogWarning("SpriteLoader: Resource path is null or empty");
            return null;
        }

        // Check cache first
        if (textureCache.TryGetValue(resourcePath, out Texture2D cached))
        {
            return Sprite.Create(
                cached,
                new Rect(0, 0, cached.width, cached.height),
                new Vector2(0.5f, 0.5f),
                pixelsPerUnit
            );
        }

        // Load on-demand
        Texture2D texture = Resources.Load<Texture2D>(resourcePath);
        if (texture != null)
        {
            textureCache[resourcePath] = texture;
            return Sprite.Create(
                texture,
                new Rect(0, 0, texture.width, texture.height),
                new Vector2(0.5f, 0.5f),
                pixelsPerUnit
            );
        }

        Debug.LogWarning($"SpriteLoader: Texture not found: {resourcePath}");
        return null;
    }

    /// <summary>
    /// Checks if a sprite exists (tries to load if not cached).
    /// </summary>
    public bool HasSprite(string resourcePath)
    {
        return GetSprite(resourcePath) != null;
    }

    /// <summary>
    /// Checks if a texture exists (tries to load if not cached).
    /// </summary>
    public bool HasTexture(string resourcePath)
    {
        if (textureCache.ContainsKey(resourcePath))
            return true;
        return Resources.Load<Texture2D>(resourcePath) != null;
    }

    /// <summary>
    /// Gets all previously-loaded sprite paths (only returns cached items).
    /// </summary>
    public IEnumerable<string> GetAllSpritePaths()
    {
        return spriteCache.Keys;
    }

    /// <summary>
    /// Gets all previously-loaded texture paths (only returns cached items).
    /// </summary>
    public IEnumerable<string> GetAllTexturePaths()
    {
        return textureCache.Keys;
    }


    #endregion

    #region Private Methods

    /// <summary>
    /// Attempts to load a sprite slice from a multi-sprite texture.
    /// Parses "Folder/texture_sliceName" to load all sprites from "Folder/texture"
    /// and caches all slices for efficiency.
    /// </summary>
    private Sprite TryLoadMultiSpriteSlice(string resourcePath)
    {
        // Find the last underscore to split texture name from slice name
        int lastSlash = resourcePath.LastIndexOf('/');
        string fileName = lastSlash >= 0 ? resourcePath.Substring(lastSlash + 1) : resourcePath;
        string directory = lastSlash >= 0 ? resourcePath.Substring(0, lastSlash) : "";

        int lastUnderscore = fileName.LastIndexOf('_');
        if (lastUnderscore <= 0)
        {
            return null; // No underscore or starts with underscore - not a slice
        }

        string textureName = fileName.Substring(0, lastUnderscore);
        string sliceName = fileName; // The full sprite name (e.g., "fishes_8")
        string texturePath = string.IsNullOrEmpty(directory) ? textureName : $"{directory}/{textureName}";

        // Load all sprites from the texture
        Sprite[] allSprites = Resources.LoadAll<Sprite>(texturePath);
        if (allSprites == null || allSprites.Length == 0)
        {
            return null;
        }

        // Cache all loaded sprites and find the requested one
        Sprite result = null;
        foreach (Sprite s in allSprites)
        {
            string spritePath = string.IsNullOrEmpty(directory) ? s.name : $"{directory}/{s.name}";
            spriteCache[spritePath] = s;

            if (s.name == sliceName)
            {
                result = s;
            }
        }

        return result;
    }

    #endregion
}
