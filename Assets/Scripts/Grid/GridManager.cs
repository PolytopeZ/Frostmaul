using System;
using UnityEngine;
public class GridManager : MonoBehaviour
{
    [SerializeField] private GridData _gridData;

    private CellState[,] _cells;

    public event Action<Vector2Int, CellState> OnCellStateChanged;

    public int Columns => _gridData.Columns;
    public int Rows => _gridData.Rows;

    private void Awake()
    {
        InitializeGrid();
    }

    // ── Initialization ───────────────────────────────────────────────────────
    private void InitializeGrid()
    {
        _cells = new CellState[_gridData.Columns, _gridData.Rows];

        // Default: all cells are buildable
        for (int col = 0; col < _gridData.Columns; col++)
        {
            for (int row = 0; row < _gridData.Rows; row++)
            {
                _cells[col, row] = CellState.Empty;
            }
        }

        SetCellStateInternal(new Vector2Int(Constants.GridEntryColumn, Constants.GridEntryRow), CellState.Blocked);
        SetCellStateInternal(new Vector2Int(Constants.GridExitColumn, Constants.GridExitRow), CellState.Blocked);

        foreach (Vector2Int cell in _gridData.BlockedCells)
        {
            SetCellStateInternal(cell, CellState.Blocked);
        }
    }

    // ── Public API ───────────────────────────────────────────────────────────
    public CellState GetCellState(Vector2Int cell)
    {
        if (!IsInBounds(cell)) return CellState.Blocked;
        return _cells[cell.x, cell.y];
    }

    public bool IsBuildable(Vector2Int cell)
    {
        return IsInBounds(cell) && _cells[cell.x, cell.y] == CellState.Empty;
    }

    public bool IsInBounds(Vector2Int cell)
    {
        return cell.x >= 0 && cell.x < _gridData.Columns
            && cell.y >= 0 && cell.y < _gridData.Rows;
    }

    public void SetCellState(Vector2Int cell, CellState newState)
    {
        if (!IsInBounds(cell)) return;
        SetCellStateInternal(cell, newState);
    }

    // ── Coordinate Conversion ────────────────────────────────────────────────
    public Vector3 CellToWorld(Vector2Int cell)
    {
        float x = transform.position.x + (cell.x * Constants.CellSize) + (Constants.CellSize * 0.5f);
        float y = transform.position.y - (cell.y * Constants.CellSize) - (Constants.CellSize * 0.5f);
        return new Vector3(x, y, 0f);
    }

    public Vector2Int WorldToCell(Vector3 worldPos)
    {
        int col = Mathf.FloorToInt((worldPos.x - transform.position.x) / Constants.CellSize);
        int row = Mathf.FloorToInt((transform.position.y - worldPos.y) / Constants.CellSize);
        Vector2Int cell = new Vector2Int(col, row);
        return IsInBounds(cell) ? cell : new Vector2Int(-1, -1);
    }

    // ── Internal ─────────────────────────────────────────────────────────────
    private void SetCellStateInternal(Vector2Int cell, CellState newState)
    {
        if (!IsInBounds(cell)) return;
        _cells[cell.x, cell.y] = newState;
        OnCellStateChanged?.Invoke(cell, newState);
    }

    // ── Editor Gizmos ────────────────────────────────────────────────────────
#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (_gridData == null) return;

        for (int col = 0; col < _gridData.Columns; col++)
        {
            for (int row = 0; row < _gridData.Rows; row++)
            {
                Vector2Int cell = new Vector2Int(col, row);
                Vector3 center = CellToWorld(cell);

                bool isEntryExit = (col == Constants.GridEntryColumn && row == Constants.GridEntryRow)
                                || (col == Constants.GridExitColumn && row == Constants.GridExitRow);

                bool isDesignerBlocked = false;
                foreach (Vector2Int blocked in _gridData.BlockedCells)
                {
                    if (blocked == cell) { isDesignerBlocked = true; break; }
                }

                if (isEntryExit || isDesignerBlocked)
                    Gizmos.color = new Color(1f, 0.2f, 0.2f, 0.45f);
                else
                    Gizmos.color = new Color(1f, 1f, 1f, 0.1f);

                Gizmos.DrawCube(center, Vector3.one * (Constants.CellSize * 0.95f));

                Gizmos.color = new Color(1f, 1f, 1f, 0.35f);
                Gizmos.DrawWireCube(center, Vector3.one * Constants.CellSize);
            }
        }
    }
#endif
}
