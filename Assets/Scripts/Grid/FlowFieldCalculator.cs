using System;
using System.Collections.Generic;
using UnityEngine;

public class FlowFieldCalculator : MonoBehaviour
{
    [SerializeField] private GridManager _gridManager;

    private int[,] _integrationField;
    private Vector2[,] _directionField;

    private static readonly Vector2Int[] s_CardinalDirs =
    {
        new Vector2Int( 1,  0),
        new Vector2Int(-1,  0),
        new Vector2Int( 0,  1),
        new Vector2Int( 0, -1),
    };

    public event Action OnFlowFieldUpdated;

    // ── Unity Lifecycle ───────────────────────────────────────────────────────

    private void Awake()
    {
        _integrationField = new int[_gridManager.Columns, _gridManager.Rows];
        _directionField = new Vector2[_gridManager.Columns, _gridManager.Rows];
    }

    private void OnEnable()
    {
        _gridManager.OnCellStateChanged += HandleCellStateChanged;
    }

    private void Start()
    {
        Compute(); // all GridManager.Awake calls are done before any Start
    }

    private void OnDisable()
    {
        _gridManager.OnCellStateChanged -= HandleCellStateChanged;
    }

    // ── Public API ───────────────────────────────────────────────────────────

    public Vector2 GetDirection(Vector2Int cell)
    {
        if (!_gridManager.IsInBounds(cell)) return Vector2.zero;
        return _directionField[cell.x, cell.y];
    }

    public bool IsReachable(Vector2Int cell)
    {
        if (!_gridManager.IsInBounds(cell)) return false;
        return _integrationField[cell.x, cell.y] != int.MaxValue;
    }

    public List<Vector2Int> GetPath()
    {
        if (_directionField == null) return new List<Vector2Int>();

        var path = new List<Vector2Int>();
        Vector2Int current = new Vector2Int(Constants.GridEntryColumn, Constants.GridEntryRow);
        Vector2Int exit = new Vector2Int(Constants.GridExitColumn, Constants.GridExitRow);
        int maxSteps = _gridManager.Columns * _gridManager.Rows;

        for (int step = 0; step < maxSteps; step++)
        {
            path.Add(current);
            if (current == exit) break;

            Vector2 dir = _directionField[current.x, current.y];
            if (dir == Vector2.zero) break;

            int dc = Mathf.RoundToInt(dir.x);
            int dr = -Mathf.RoundToInt(dir.y);
            current = new Vector2Int(current.x + dc, current.y + dr);
        }

        return path;
    }


    // ── Event Handler ────────────────────────────────────────────────────────

    private void HandleCellStateChanged(Vector2Int cell, CellState newState)
    {
        if (newState == CellState.Tower || newState == CellState.Empty)
            Compute();
    }

    // ── Computation ──────────────────────────────────────────────────────────

    private void Compute()
    {
        ComputeIntegrationField();
        ComputeDirectionField();
        OnFlowFieldUpdated?.Invoke();
    }

    private void ComputeIntegrationField()
    {
        for (int col = 0; col < _gridManager.Columns; col++)
            for (int row = 0; row < _gridManager.Rows; row++)
                _integrationField[col, row] = int.MaxValue;

        Vector2Int exit = new Vector2Int(Constants.GridExitColumn, Constants.GridExitRow);
        _integrationField[exit.x, exit.y] = 0;

        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        queue.Enqueue(exit);

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();
            int nextDist = _integrationField[current.x, current.y] + 1;

            for (int i = 0; i < s_CardinalDirs.Length; i++)
            {
                Vector2Int neighbor = current + s_CardinalDirs[i];
                if (!_gridManager.IsInBounds(neighbor)) continue;
                if (_integrationField[neighbor.x, neighbor.y] != int.MaxValue) continue;
                if (_gridManager.GetCellState(neighbor) == CellState.Tower) continue;

                _integrationField[neighbor.x, neighbor.y] = nextDist;
                queue.Enqueue(neighbor);
            }
        }
    }

    private void ComputeDirectionField()
    {
        for (int col = 0; col < _gridManager.Columns; col++)
        {
            for (int row = 0; row < _gridManager.Rows; row++)
            {
                Vector2Int cell = new Vector2Int(col, row);

                if (_gridManager.GetCellState(cell) == CellState.Tower ||
                    _integrationField[col, row] == int.MaxValue)
                {
                    _directionField[col, row] = Vector2.zero;
                    continue;
                }

                Vector2Int best = cell;
                int bestDist = _integrationField[col, row];

                for (int i = 0; i < s_CardinalDirs.Length; i++)
                {
                    Vector2Int neighbor = cell + s_CardinalDirs[i];
                    if (!_gridManager.IsInBounds(neighbor)) continue;
                    if (_integrationField[neighbor.x, neighbor.y] < bestDist)
                    {
                        bestDist = _integrationField[neighbor.x, neighbor.y];
                        best = neighbor;
                    }
                }

                if (best == cell)
                {
                    _directionField[col, row] = Vector2.zero;
                }
                else
                {
                    Vector3 from = _gridManager.CellToWorld(cell);
                    Vector3 to = _gridManager.CellToWorld(best);
                    _directionField[col, row] = ((Vector2)(to - from)).normalized;
                }
            }
        }
    }

    // ── Editor Gizmos ────────────────────────────────────────────────────────
#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (_gridManager == null || _integrationField == null) return;

        int maxDist = 0;
        for (int col = 0; col < _gridManager.Columns; col++)
            for (int row = 0; row < _gridManager.Rows; row++)
                if (_integrationField[col, row] != int.MaxValue)
                    maxDist = Mathf.Max(maxDist, _integrationField[col, row]);

        for (int col = 0; col < _gridManager.Columns; col++)
        {
            for (int row = 0; row < _gridManager.Rows; row++)
            {
                Vector2Int cell = new Vector2Int(col, row);
                Vector3 center = _gridManager.CellToWorld(cell);
                int dist = _integrationField[col, row];

                if (dist == int.MaxValue)
                {
                    Gizmos.color = new Color(1f, 0f, 0f, 0.4f);
                    Gizmos.DrawCube(center, Vector3.one * (Constants.CellSize * 0.3f));
                }
                else
                {
                    float t = maxDist > 0 ? (float)dist / maxDist : 0f;
                    Color gradientColor = Color.Lerp(Color.green, Color.blue, t);
                    gradientColor.a = 0.35f;
                    Gizmos.color = gradientColor;
                    Gizmos.DrawCube(center, Vector3.one * (Constants.CellSize * 0.25f));

                    Vector2 dir = _directionField[col, row];
                    if (dir != Vector2.zero)
                    {
                        Gizmos.color = Color.white;
                        Gizmos.DrawLine(center, center + new Vector3(dir.x, dir.y) * (Constants.CellSize * 0.4f));
                    }
                }
            }
        }
    }
#endif
}
