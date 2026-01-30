public static class CameraBounds
{
    private static float _minX;
    private static float _maxX;
    private static bool _initialized;

    public static float MinX => _minX;
    public static float MaxX => _maxX;

    public static void Initialize()
    {
        _minX = -GameSettings.WorldWidth / 2f;
        _maxX = GameSettings.WorldWidth / 2f;
        _initialized = true;
    }

    /// <summary>
    /// Checks if a position is within horizontal screen bounds.
    /// </summary>
    /// <param name="x">X position to check</param>
    /// <param name="margin">Margin from edge (for sprite width)</param>
    /// <returns>-1 if beyond left edge, 0 if within bounds, 1 if beyond right edge</returns>
    public static int CheckHorizontalBounds(float x, float margin = 0f)
    {
        if (!_initialized)
            return 0;

        if (x - margin < _minX)
            return -1;
        if (x + margin > _maxX)
            return 1;
        return 0;
    }
}
