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
        // Update character animations and state (no mana generation)
        CharacterControllers.Instance.Update(Time.deltaTime);
    }

    private void SetupGame()
    {
        new GameObject("DragDropInputSystem").AddComponent<DragDropInputSystem>();
        LevelManager.Instance.LoadLevel(LevelDefinitions.FirstLevelId);
    }

    private void SetupHUD()
    {
        HUDCanvas.Create();
    }
}
