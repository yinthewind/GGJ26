using System.Collections.Generic;
using UnityEngine;

public class WorldSpaceProductionGraph : MonoBehaviour
{
    public static WorldSpaceProductionGraph Instance { get; private set; }

    private Canvas _canvas;
    private ProductionHistoryGraph _graph;
    private List<float> _productionHistory = new List<float>();

    public static WorldSpaceProductionGraph Create(Vector3 worldPosition)
    {
        GameObject obj = new GameObject("WorldSpaceProductionGraph");
        obj.transform.position = worldPosition;
        obj.transform.rotation = Quaternion.Euler(0f, 0f, 30f);

        WorldSpaceProductionGraph component = obj.AddComponent<WorldSpaceProductionGraph>();
        component.BuildCanvas();
        component.BuildGraph();

        Debug.Log($"WorldSpaceProductionGraph created at {worldPosition}");

        return component;
    }

    private void Awake() => Instance = this;

    private void Start()
    {
        TurnManager.Instance.OnProductivityGained += HandleProductivityGained;
        LevelManager.Instance.OnLevelLoaded += HandleLevelLoaded;
    }

    private void BuildCanvas()
    {
        GameObject canvasObj = new GameObject("Canvas");
        canvasObj.transform.SetParent(transform, false);

        _canvas = canvasObj.AddComponent<Canvas>();
        _canvas.renderMode = RenderMode.WorldSpace;
        _canvas.worldCamera = Camera.main;

        // Set sorting order to render above sprites
        _canvas.sortingOrder = 100;

        RectTransform canvasRect = canvasObj.GetComponent<RectTransform>();
        canvasRect.sizeDelta = new Vector2(200f, 100f);
        // Scale: 200 UI units * 0.01 = 2 world units wide
        canvasRect.localScale = new Vector3(0.01f, 0.01f, 1f);
        canvasRect.localPosition = Vector3.zero;
    }

    private void BuildGraph()
    {
        _graph = ProductionHistoryGraph.Create(
            _canvas.transform,
            200f,   // width
            100f,   // height
            new Color(0.3f, 0.7f, 0.3f),  // green bars
            4f);    // spacing

        RectTransform graphRect = _graph.GetComponent<RectTransform>();
        graphRect.anchorMin = new Vector2(0.5f, 0.5f);
        graphRect.anchorMax = new Vector2(0.5f, 0.5f);
        graphRect.anchoredPosition = Vector2.zero;
    }

    private void HandleProductivityGained(float productivity)
    {
        _productionHistory.Add(productivity);
        _graph.SetData(_productionHistory);
        Debug.Log($"ProductionHistory updated: {productivity}, total bars: {_productionHistory.Count}");
    }

    private void HandleLevelLoaded(string levelId)
    {
        _productionHistory.Clear();
        _graph.SetData(_productionHistory);
    }

    private void OnDestroy()
    {
        if (TurnManager.Instance != null)
        {
            TurnManager.Instance.OnProductivityGained -= HandleProductivityGained;
        }

        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.OnLevelLoaded -= HandleLevelLoaded;
        }
    }
}
