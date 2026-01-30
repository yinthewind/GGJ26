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

    private void SetupGame()
    {
        SpawnInitialWorkspaces();
        SpawnInitialCharacters();
    }

    private void SpawnInitialWorkspaces()
    {
        // Spawn a few 1x1 Basic workspaces at ground level (grid Y=1)
        WorkspaceControllers.Instance.SpawnWorkspace(
            new Vector3(-2f, 1f, 0f), new Vector2Int(1, 1), WorkspaceType.Basic, "Workspace1");
        WorkspaceControllers.Instance.SpawnWorkspace(
            new Vector3(0f, 1f, 0f), new Vector2Int(1, 1), WorkspaceType.Basic, "Workspace2");
        WorkspaceControllers.Instance.SpawnWorkspace(
            new Vector3(2f, 1f, 0f), new Vector2Int(1, 1), WorkspaceType.Basic, "Workspace3");
    }

    private void SpawnInitialCharacters()
    {
        // Spawn a few skeletons above ground level (they will fall down)
        CharacterControllers.Instance.SpawnSkeleton(
            SkeletonType.Swordsman, new Vector3(-3f, 1f, 0f));
        CharacterControllers.Instance.SpawnSkeleton(
            SkeletonType.Archer, new Vector3(3f, 1f, 0f));
    }

    private void SetupHUD()
    {
        var hudCanvas = HUDCanvas.Create();
        HUDButtonContainer.Create(hudCanvas.Canvas.transform, 4);
    }
}
