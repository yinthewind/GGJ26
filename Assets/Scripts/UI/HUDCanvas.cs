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
    public ShowSynergiesButton ShowSynergiesButton { get; private set; }
    public SynergyModal SynergyModal { get; private set; }

    public static HUDCanvas Create()
    {
        GameObject obj = new GameObject("HUDCanvas");
        HUDCanvas hud = obj.AddComponent<HUDCanvas>();
        hud.BuildCanvas();
        hud.BuildUI();
        return hud;
    }

    private void Awake() => Instance = this;

    private void BuildCanvas()
    {
        GameObject canvasObj = new GameObject("Canvas");
        canvasObj.transform.SetParent(transform, false);

        Canvas = canvasObj.AddComponent<Canvas>();
        Canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObj.AddComponent<UnityEngine.UI.CanvasScaler>();
        canvasObj.AddComponent<UnityEngine.UI.GraphicRaycaster>();
    }

    private void BuildUI()
    {
        // Create Turn Counter (top-left)
        TurnCounter = TurnCounter.Create(Canvas.transform, 100f, 40f);

        // Create Productivity Panel (top-center)
        ProductivityPanel = ProductivityPanel.Create(Canvas.transform, 180f, 60f);

        // Create Goal Panel (top-right)
        GoalPanel = GoalPanel.Create(Canvas.transform, 200f, 80f);

        // Create End Turn Button (bottom-right)
        EndTurnButton = EndTurnButton.Create(Canvas.transform, 140f, 50f);

        // Create Synergy Panel (bottom-left)
        SynergyPanel = SynergyPanel.Create(Canvas.transform, 200f, 320f);

        // Create Show Synergies Button (bottom-right, above End Turn)
        ShowSynergiesButton = ShowSynergiesButton.Create(Canvas.transform, 140f, 40f);

        // Create Synergy Modal (hidden by default, on top of everything)
        SynergyModal = SynergyModal.Create(Canvas.transform);
        ShowSynergiesButton.BindModal(SynergyModal);
    }
}
