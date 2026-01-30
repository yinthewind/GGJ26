using UnityEngine;

public class HUDCanvas : MonoBehaviour
{
    public static HUDCanvas Instance { get; private set; }
    public Canvas Canvas { get; private set; }

    public static HUDCanvas Create()
    {
        GameObject obj = new GameObject("HUDCanvas");
        HUDCanvas hud = obj.AddComponent<HUDCanvas>();
        hud.BuildCanvas();
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
}
