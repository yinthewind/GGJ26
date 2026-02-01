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
        CreateBackground();
        FloorGridAnimator.Create();
        new GameObject("DragDropInputSystem").AddComponent<DragDropInputSystem>();
        LevelManager.Instance.LoadLevel(LevelDefinitions.FirstLevelId);

        MusicManager.Create();
        MusicManager.Instance.PlayMusic("Sounds/Late-at-Night(chosic.com)");
    }

    private void CreateBackground()
    {
        GameObject bgObj = new GameObject("Background");
        Vector3 camPos = Camera.main.transform.position;
        bgObj.transform.position = new Vector3(camPos.x, camPos.y, 0f);
        SpriteRenderer sr = bgObj.AddComponent<SpriteRenderer>();
        sr.sprite = SpriteLoader.Instance.GetSprite("Sprites/UIUX/BACKGROUND");
        sr.sortingOrder = -3;
    }

    private void SetupHUD()
    {
        HUDCanvas.Create();
        WorldSpaceProductionGraph.Create(new Vector3(4f, 3f, 0f));
    }
}
