using UnityEngine;
using UnityEngine.InputSystem;

public class TowerPlacer : MonoBehaviour
{
    [SerializeField] private GridManager _gridManager;
    [SerializeField] private GameObject _highlight;
    [SerializeField] private PathValidator _pathValidator;
    [SerializeField] private TowerData _towerToPlace;



    private FrostmaulInput _input;
    private Vector2Int _selectedCell = new Vector2Int(-1, -1);
    private bool _hasSelection;

    private void Awake()
    {
        _input = new FrostmaulInput();
    }

    private void OnEnable()
    {
        _input.Enable();
        _input.Gameplay.Tap.performed += OnTap;
    }

    private void OnDisable()
    {
        _input.Gameplay.Tap.performed -= OnTap;
        _input.Disable();
    }

    private void OnTap(InputAction.CallbackContext ctx)
    {
        if (CameraScroller.IsDragging) return;

        Vector2 screenPos = _input.Gameplay.PointerPosition.ReadValue<Vector2>();
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, 0f));
        worldPos.z = 0f;

        Vector2Int cell = _gridManager.WorldToCell(worldPos);

        if (cell.x == Constants.InvalidCell)
        {
            ClearSelection();
            return;
        }

        if (_hasSelection && cell == _selectedCell)
        {
            PlaceTower(cell);
        }
        else if (_gridManager.IsBuildable(cell))
        {
            SelectCell(cell);
        }
        else
        {
            ClearSelection();
        }
    }

    private void SelectCell(Vector2Int cell)
    {
        _selectedCell = cell;
        _hasSelection = true;
        _highlight.SetActive(true);
        _highlight.transform.position = _gridManager.CellToWorld(cell);
    }

    private void PlaceTower(Vector2Int cell)
    {
        if (!_pathValidator.IsPlacementValid(cell)) return;
        if (_towerToPlace == null || _towerToPlace.Prefab == null) return;

        _gridManager.SetCellState(cell, CellState.Tower);
        Instantiate(_towerToPlace.Prefab, _gridManager.CellToWorld(cell), Quaternion.identity);
        ClearSelection();
    }



    private void ClearSelection()
    {
        _hasSelection = false;
        _selectedCell = new Vector2Int(-1, -1);
        _highlight.SetActive(false);
    }

    public void SetTowerData(TowerData data)
    {
        _towerToPlace = data;
    }

}
