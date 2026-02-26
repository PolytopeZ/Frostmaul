using System.Collections.Generic;
using UnityEngine;

public class PathValidator : MonoBehaviour
{
    [SerializeField] private GridManager _gridManager;

    private bool[,] _visited;

    private static readonly Vector2Int[] s_CardinalDirs =
    {
        new Vector2Int( 1,  0),
        new Vector2Int(-1,  0),
        new Vector2Int( 0,  1),
        new Vector2Int( 0, -1),
    };

    private void Awake()
    {
        _visited = new bool[_gridManager.Columns, _gridManager.Rows];
    }

    public bool IsPlacementValid(Vector2Int candidateCell)
    {
        System.Array.Clear(_visited, 0, _visited.Length);

        Vector2Int exit = new Vector2Int(Constants.GridExitColumn, Constants.GridExitRow);
        Vector2Int entry = new Vector2Int(Constants.GridEntryColumn, Constants.GridEntryRow);

        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        _visited[exit.x, exit.y] = true;
        queue.Enqueue(exit);

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();
            if (current == entry) return true;

            for (int i = 0; i < s_CardinalDirs.Length; i++)
            {
                Vector2Int neighbor = current + s_CardinalDirs[i];
                if (!_gridManager.IsInBounds(neighbor)) continue;
                if (_visited[neighbor.x, neighbor.y]) continue;
                if (_gridManager.GetCellState(neighbor) == CellState.Tower) continue;
                if (neighbor == candidateCell) continue;

                _visited[neighbor.x, neighbor.y] = true;
                queue.Enqueue(neighbor);
            }
        }

        return false;
    }
}
