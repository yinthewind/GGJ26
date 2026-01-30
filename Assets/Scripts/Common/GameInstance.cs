using UnityEngine;

public class GameInstance : MonoBehaviour
{
    public static GameInstance Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        SetupGame();
        SetupHUD();
    }

    private void Update()
    {
        CharacterControllers.Instance.Update(Time.deltaTime);
    }

    private void SetupGame()
    {
        new GameObject("DragDropInputSystem").AddComponent<DragDropInputSystem>();
        SpawnInitialWorkspaces();
        SpawnInitialCharacters();
    }

    private void SpawnInitialWorkspaces()
    {
        // Spawn 3 1x1 Basic workspaces in L (Gamma) shape
        // Bottom-left
        WorkspaceControllers.Instance.SpawnWorkspace(
            new Vector3(0f, 1f, 0f), new Vector2Int(1, 1), WorkspaceType.Basic, "Workspace1");
        // Bottom-right
        WorkspaceControllers.Instance.SpawnWorkspace(
            new Vector3(1f, 1f, 0f), new Vector2Int(1, 1), WorkspaceType.Basic, "Workspace2");
        // Top-right (stacked on bottom-right)
        WorkspaceControllers.Instance.SpawnWorkspace(
            new Vector3(1f, 2f, 0f), new Vector2Int(1, 1), WorkspaceType.Basic, "Workspace3");
    }

    private void SpawnInitialCharacters()
    {
        // Spawn a few skeletons above ground level (they will fall down)
        CharacterControllers.Instance.SpawnSkeleton(
            WorkhorseType.Swordsman, new Vector3(-3f, 1f, 0f));
        CharacterControllers.Instance.SpawnSkeleton(
            WorkhorseType.Archer, new Vector3(3f, 1f, 0f));
    }

    private void SetupHUD()
    {
        var hudCanvas = HUDCanvas.Create();
        HUDButtonContainer.Create(hudCanvas.Canvas.transform, 4);
    }
}
