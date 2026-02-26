# Grid T1 — 9×16 Tilemap Grid with Cell State Flags

## What & Why

This task establishes the **logical foundation of the game grid** — the single most
important data structure in Frostmaul. Every other system (towers, pathfinding, enemies,
card effects) reads from or writes to this grid.

We separate responsibilities into two distinct concerns:

| Concern                                   | Owner                                    |
| ----------------------------------------- | ---------------------------------------- |
| **Logic** — who owns what cell            | `GridManager.cs`                         |
| **Configuration** — how many columns/rows | `GridData` ScriptableObject              |
| **Visualization** — what tiles are drawn  | Unity Tilemap (wired up in a later task) |

Why not use the Tilemap directly as the source of truth? Because Tilemap is a rendering
component. Querying it for game logic (is this cell buildable?) is slow, brittle, and
couples rendering to gameplay. `GridManager` holds a plain `CellState[,]` array — fast,
allocation-free lookups in O(1).

Why a ScriptableObject for configuration? The grid dimensions and blocked cells are
designer-tunable data, not programmer constants. A `GridData` SO can be swapped per
level or run-type without touching code.

---

## Prerequisites

- Unity 6 project with URP 2D configured (M1 T1 done)
- `Assets/Scripts/Utils/`, `Assets/Scripts/Grid/`, `Assets/Data/` folders exist ✓
- 2D Tilemap package installed (comes with URP 2D template — verify in Package Manager)

---

## Step 1 — Write the Global Constants

**Why:** Architecture rule #5 forbids magic numbers in code. All grid sizing constants
must live in a single, findable file.

Create `Assets/Scripts/Utils/Constants.cs` with the content from the
[Constants.cs code block](#constantscs) below.

Key values being defined:
- `GridColumns = 9`, `GridRows = 16` — the base grid size
- `CellSize = 1f` — 1 Unity unit per cell (matches Tilemap default cell size)
- `GridEntryColumn / GridEntryRow` — where enemies enter (top-center)
- `GridExitColumn / GridExitRow` — where enemies exit (bottom-center)

---

## Step 2 — Define the CellState Enum

**Why:** Every cell in the grid can be in one of four states. An enum makes state
checks exhaustive, readable, and switch-safe (compiler warns on missing cases).

Create `Assets/Scripts/Grid/CellState.cs` with the content from the
[CellState.cs code block](#cellstatecs) below.

State meanings:
| State     | Meaning                                                                       |
| --------- | ----------------------------------------------------------------------------- |
| `Empty`   | Buildable. Player can place a tower here.                                     |
| `Tower`   | Occupied. A tower is present. Blocks enemy pathfinding.                       |
| `Path`    | Part of the live enemy route (used for visual hints, set by FlowField later). |
| `Blocked` | Can never be built on — entry cell, exit cell, or designer-marked.            |

---

## Step 3 — Create the GridData ScriptableObject

**Why:** Grid dimensions and any designer-defined blocked cells (borders, obstacles) are
data, not code. Putting them in a SO means a designer can add a new blocked cell without
touching a `.cs` file.

Create `Assets/Scripts/Grid/GridData.cs` with the content from the
[GridData.cs code block](#griddatacs) below.

---

## Step 4 — Create the GridManager MonoBehaviour

**Why:** `GridManager` is the runtime owner of the cell state array. It exposes a clean
public API so no other system ever directly indexes the array.

Create `Assets/Scripts/Grid/GridManager.cs` with the content from the
[GridManager.cs code block](#gridmanagercs) below.

Key design decisions baked into `GridManager`:
- **`transform.position` = top-left corner of the grid.** Placing the GameObject in the
  scene directly controls where the grid appears. No hidden offsets.
- **Row 0 is the top of the grid.** Row increases downward, matching the direction of
  enemy travel. `CellToWorld` accounts for the Unity y-up flip.
- **`OnCellStateChanged` event.** Fired when any cell changes state. The future
  `PathValidator`, `GridVisualizer`, and `SynergyDetector` all subscribe here. No polling,
  no direct references between systems.
- **`OnDrawGizmosSelected`** draws the full grid in the Scene view when the
  `GridManager` GameObject is selected. Essential for development — you can see exactly
  which cells are blocked before any tiles are painted.

---

## Step 5 — Set Up the Tilemap in the Game Scene

The Tilemap is not yet used by `GridManager` for logic, but it must exist in the scene
now so that future tasks (tile painting, grid visualization) have a target to write to.

1. In Unity, open `Assets/Scenes/Game.unity`
2. In the Hierarchy, right-click the root → **2D Object → Tilemap → Rectangular**
   Unity creates two objects: `Grid` (parent) and `Tilemap` (child)
3. Rename `Grid` → **`GameGrid`**
4. Rename `Tilemap` → **`GridTilemap`**
5. Select **`GameGrid`** → Inspector → set:

   | Field       | Value                |
   | ----------- | -------------------- |
   | Cell Size   | X: `1` Y: `1` Z: `0` |
   | Cell Gap    | X: `0` Y: `0` Z: `0` |
   | Cell Layout | Rectangle            |

6. Select **`GridTilemap`** → Inspector → set:
   - **Order in Layer:** `0` (background; towers will be higher)

> **Note:** Leave the Tilemap empty for now. Visual tile painting is a separate task.

---

## Step 6 — Create the GridData Asset

1. In the Project window, navigate to `Assets/Data/`
2. Right-click → **Create → Frostmaul → Grid → GridData**
3. Name it `BaseGridData`
4. Select it → Inspector → confirm:

   | Field         | Value                   |
   | ------------- | ----------------------- |
   | Columns       | `9`                     |
   | Rows          | `16`                    |
   | Blocked Cells | *(leave empty for now)* |

---

## Step 7 — Wire Up GridManager in the Scene

1. In the Hierarchy, right-click → **Create Empty** → rename to **`GridManager`**
2. Set its **Transform Position** to the desired top-left corner of the grid.
   For a 9-unit wide grid centered at X=0: **Position = (-4.5, 8, 0)**
   *(9 columns × 1 unit = 9 units wide; centered → left edge at -4.5. 16 rows tall → top at y=8, bottom at y=-8.)*
3. With `GridManager` selected → **Add Component → Scripts → Frostmaul.Grid → GridManager**
4. In the Inspector, wire up:

   | Field     | Value                                   |
   | --------- | --------------------------------------- |
   | Grid Data | Drag `BaseGridData` from `Assets/Data/` |

---

## Step 8 — Verify in the Editor

1. Select the `GridManager` GameObject in the Hierarchy
2. In the Scene view, you should see the Gizmo overlay:
   - **White lines** — the full 9×16 grid outline
   - **Red cells** — the entry cell (top-center) and exit cell (bottom-center)
3. Enter Play mode → no errors in the Console
4. In the Console (or via a temporary test script), call
   `gridManager.IsBuildable(new Vector2Int(4, 0))` — should return `false` (entry cell is Blocked)
5. Call `gridManager.IsBuildable(new Vector2Int(0, 1))` — should return `true` (empty cell)

---

## Code

### Constants.cs

```csharp
// Assets/Scripts/Utils/Constants.cs

/// <summary>
/// Global constants for the Frostmaul project.
/// No magic numbers anywhere else in the codebase — all values live here.
/// </summary>
public static class Constants
{
    // ── Grid ─────────────────────────────────────────────────────────────────
    public const int GridColumns = 9;
    public const int GridRows = 16;
    public const float CellSize = 1f;

    // Entry cell: top-center (row 0, column 4)
    public const int GridEntryRow = 0;
    public const int GridEntryColumn = GridColumns / 2; // = 4

    // Exit cell: bottom-center (row 15, column 4)
    public const int GridExitRow = GridRows - 1;        // = 15
    public const int GridExitColumn = GridColumns / 2;  // = 4
}
```

### CellState.cs

```csharp
// Assets/Scripts/Grid/CellState.cs

/// <summary>
/// Represents the logical state of a single cell on the game grid.
/// </summary>
public enum CellState
{
    /// <summary>Cell is empty and available for tower placement.</summary>
    Empty,

    /// <summary>Cell is occupied by a tower and blocks enemy pathfinding.</summary>
    Tower,

    /// <summary>
    /// Cell is part of the current enemy route.
    /// Set by FlowFieldCalculator; used for visual hints only.
    /// </summary>
    Path,

    /// <summary>
    /// Cell cannot be built on (entry, exit, or designer-defined obstacle).
    /// </summary>
    Blocked,
}
```

### GridData.cs

```csharp
// Assets/Scripts/Grid/GridData.cs
using UnityEngine;

/// <summary>
/// ScriptableObject that holds static configuration for the game grid.
/// One asset per layout variant. The base game uses a single 9×16 layout.
/// </summary>
[CreateAssetMenu(fileName = "GridData", menuName = "Frostmaul/Grid/GridData")]
public class GridData : ScriptableObject
{
    [Header("Dimensions")]
    [SerializeField] private int _columns = 9;
    [SerializeField] private int _rows = 16;

    [Header("Designer-Blocked Cells")]
    [Tooltip("Cells that are permanently Blocked regardless of game state.")]
    [SerializeField] private Vector2Int[] _blockedCells = new Vector2Int[0];

    public int Columns => _columns;
    public int Rows => _rows;
    public Vector2Int[] BlockedCells => _blockedCells;
}
```

### GridManager.cs

```csharp
// Assets/Scripts/Grid/GridManager.cs
using System;
using UnityEngine;

/// <summary>
/// Manages the logical state of the game grid.
/// Owns the CellState[columns, rows] array and exposes the public API
/// used by towers, pathfinding, and UI systems.
///
/// transform.position = top-left corner of the grid in world space.
/// Row 0 = top (enemy entry). Row increases downward.
/// </summary>
public class GridManager : MonoBehaviour
{
    [SerializeField] private GridData _gridData;

    private CellState[,] _cells;

    /// <summary>Fired when any cell changes state. Args: cell coords, new state.</summary>
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

        for (int col = 0; col < _gridData.Columns; col++)
            for (int row = 0; row < _gridData.Rows; row++)
                _cells[col, row] = CellState.Empty;

        SetCellStateInternal(new Vector2Int(Constants.GridEntryColumn, Constants.GridEntryRow), CellState.Blocked);
        SetCellStateInternal(new Vector2Int(Constants.GridExitColumn, Constants.GridExitRow), CellState.Blocked);

        foreach (Vector2Int cell in _gridData.BlockedCells)
            SetCellStateInternal(cell, CellState.Blocked);
    }

    // ── Public API ───────────────────────────────────────────────────────────

    /// <summary>Returns the current state of the given grid cell.</summary>
    public CellState GetCellState(Vector2Int cell)
    {
        if (!IsInBounds(cell)) return CellState.Blocked;
        return _cells[cell.x, cell.y];
    }

    /// <summary>Returns true if a tower can be placed at this cell.</summary>
    public bool IsBuildable(Vector2Int cell)
    {
        return IsInBounds(cell) && _cells[cell.x, cell.y] == CellState.Empty;
    }

    /// <summary>Returns true if the cell is within the grid bounds.</summary>
    public bool IsInBounds(Vector2Int cell)
    {
        return cell.x >= 0 && cell.x < _gridData.Columns
            && cell.y >= 0 && cell.y < _gridData.Rows;
    }

    /// <summary>
    /// Sets the state of a cell and fires OnCellStateChanged.
    /// Called by TowerPlacer, PathValidator, FlowFieldCalculator, etc.
    /// </summary>
    public void SetCellState(Vector2Int cell, CellState newState)
    {
        if (!IsInBounds(cell)) return;
        SetCellStateInternal(cell, newState);
    }

    // ── Coordinate Conversion ────────────────────────────────────────────────

    /// <summary>
    /// Converts a logical grid cell (col, row) to world-space center position.
    /// Row 0 is at the top; row increases downward (Unity y decreases).
    /// </summary>
    public Vector3 CellToWorld(Vector2Int cell)
    {
        float x = transform.position.x + (cell.x * Constants.CellSize) + (Constants.CellSize * 0.5f);
        float y = transform.position.y - (cell.y * Constants.CellSize) - (Constants.CellSize * 0.5f);
        return new Vector3(x, y, 0f);
    }

    /// <summary>
    /// Converts a world-space position to a logical grid cell.
    /// Returns (-1, -1) if the position is outside the grid.
    /// </summary>
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
```

---

## ✅ This Task Is Done When

- [x] `Assets/Scripts/Utils/Constants.cs` compiles with no errors
- [x] `Assets/Scripts/Grid/CellState.cs` compiles with no errors
- [x] `Assets/Scripts/Grid/GridData.cs` compiles and the Create menu shows **Frostmaul > Grid > GridData**
- [x] `Assets/Scripts/Grid/GridManager.cs` compiles with no errors
- [x] `BaseGridData` asset exists at `Assets/Data/BaseGridData.asset`
- [x] `GameGrid` + `GridTilemap` hierarchy exists in `Game.unity`
- [x] `GridManager` GameObject exists in `Game.unity`, wired to `BaseGridData`
- [x] Selecting `GridManager` in the Scene view shows the white + red Gizmo grid
- [x] Play mode produces no Console errors

---

## Next Task

→ **Grid T2 — BFS PathValidator** (ensure a valid path exists before confirming tower placement)
