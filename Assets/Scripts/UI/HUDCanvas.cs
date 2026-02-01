using UnityEngine;

public class HUDCanvas : MonoBehaviour
{
    public static HUDCanvas Instance { get; private set; }
    public Canvas Canvas { get; private set; }

    // UI Components
    public EndTurnButton EndTurnButton { get; private set; }
    public ProductivityPanel ProductivityPanel { get; private set; }
    public TurnCounter TurnCounter { get; private set; }
    public GoalPanel GoalPanel { get; private set; }
    public SynergyPanel SynergyPanel { get; private set; }
    public SynergyModal SynergyModal { get; private set; }
    public LevelResultModal LevelResultModal { get; private set; }
    public WorkhorseShopPanel ShopPanel { get; private set; }
    public WorkspaceHoverTooltip HoverTooltip { get; private set; }
    public MoneyPanel MoneyPanel { get; private set; }

    public static HUDCanvas Create()
    {
        GameObject obj = new GameObject("HUDCanvas");
        HUDCanvas hud = obj.AddComponent<HUDCanvas>();
        hud.BuildCanvas();
        hud.BuildUI();
        return hud;
    }

    private void Awake() => Instance = this;

    private void Start()
    {
        // Subscribe to level events
        LevelManager.Instance.OnLevelWon += HandleLevelWon;
        LevelManager.Instance.OnLevelFailed += HandleLevelFailed;
    }

    private void BuildCanvas()
    {
        GameObject canvasObj = new GameObject("Canvas");
        canvasObj.transform.SetParent(transform, false);

        Canvas = canvasObj.AddComponent<Canvas>();
        Canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObj.AddComponent<UnityEngine.UI.CanvasScaler>();
        canvasObj.AddComponent<UnityEngine.UI.GraphicRaycaster>();
    }

    private Canvas BuildPixelPerfectCanvas(string name)
    {
        GameObject canvasObj = new GameObject(name);
        canvasObj.transform.SetParent(transform, false);

        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        var scaler = canvasObj.AddComponent<UnityEngine.UI.CanvasScaler>();
        scaler.uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ConstantPixelSize;

        canvasObj.AddComponent<UnityEngine.UI.GraphicRaycaster>();

        return canvas;
    }

    private void BuildUI()
    {
        // Create Turn Counter (top-left)
        TurnCounter = TurnCounter.Create(Canvas.transform, 170f, 40f);

        // Create Money Panel (below TurnCounter)
        MoneyPanel = MoneyPanel.Create(Canvas.transform, 160f, 40f);

        // Create Productivity Panel (top-center)
        ProductivityPanel = ProductivityPanel.Create(Canvas.transform, 180f, 60f);

        // Create Goal Panel (top-right)
        GoalPanel = GoalPanel.Create(Canvas.transform, 200f, 80f);

        // Create End Turn Button (bottom-right corner)
        EndTurnButton = EndTurnButton.Create(Canvas.transform, () => TurnManager.Instance.EndTurn());
        RectTransform endTurnRect = EndTurnButton.GetComponent<RectTransform>();
        endTurnRect.anchorMin = new Vector2(1, 0);
        endTurnRect.anchorMax = new Vector2(1, 0);
        endTurnRect.pivot = new Vector2(1, 0);
        endTurnRect.anchoredPosition = new Vector2(-20, 20);

        // Create Synergy Panel (bottom-left) on pixel-perfect canvas
        Canvas synergyCanvas = BuildPixelPerfectCanvas("SynergyCanvas");
        SynergyPanel = SynergyPanel.Create(synergyCanvas.transform, 256f, 488f);

        // Create Synergy Modal (hidden by default, on top of everything)
        SynergyModal = SynergyModal.Create(Canvas.transform);

        // Create Level Result Modal (hidden by default, on top of everything)
        LevelResultModal = LevelResultModal.Create(Canvas.transform);

        // Create Workhorse Shop Panel (always visible, right side)
        Sprite shopSprite = SpriteLoader.Instance.GetSprite("Sprites/UIUX/Shop");
        float shopWidth = shopSprite.rect.width;
        float shopHeight = shopSprite.rect.height;
        ShopPanel = WorkhorseShopPanel.Create(
            Canvas.transform,
            shopWidth,
            shopHeight,
            GameSettings.TotalShopSlots,
            GameSettings.InitialActiveSlots);

        // Bind synergy modal to shop panel's synergies button
        ShopPanel.BindSynergyModal(SynergyModal);

        // Create Hover Tooltip (follows mouse, on top of everything)
        HoverTooltip = WorkspaceHoverTooltip.Create(Canvas.transform);
    }

    private void HandleLevelWon()
    {
        LevelResultModal.ShowWin();
    }

    private void HandleLevelFailed()
    {
        LevelResultModal.ShowFail();
    }

    private void OnDestroy()
    {
        LevelManager.Instance.OnLevelWon -= HandleLevelWon;
        LevelManager.Instance.OnLevelFailed -= HandleLevelFailed;
    }
}
