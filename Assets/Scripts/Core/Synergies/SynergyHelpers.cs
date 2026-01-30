using System.Collections.Generic;
using UnityEngine;

public static class SynergyHelpers
{
    public static bool AreWorkspacesAdjacent(Workspace w1, Workspace w2)
    {
        var cells1 = GetOccupiedCells(w1);
        var cells2 = GetOccupiedCells(w2);

        foreach (var c1 in cells1)
        {
            foreach (var c2 in cells2)
            {
                if (AreCellsAdjacent(c1, c2))
                    return true;
            }
        }

        return false;
    }

    public static List<Vector2Int> GetOccupiedCells(Workspace workspace)
    {
        var cells = new List<Vector2Int>();
        for (int x = 0; x < workspace.Size.x; x++)
        {
            for (int y = 0; y < workspace.Size.y; y++)
            {
                cells.Add(new Vector2Int(workspace.Position.x + x, workspace.Position.y + y));
            }
        }
        return cells;
    }

    public static bool AreCellsAdjacent(Vector2Int c1, Vector2Int c2)
    {
        int dx = Mathf.Abs(c1.x - c2.x);
        int dy = Mathf.Abs(c1.y - c2.y);
        return (dx == 1 && dy == 0) || (dx == 0 && dy == 1);
    }
}
