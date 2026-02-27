using System;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;


public class TowerPlacer : MonoBehaviour
{
    [SerializeField] private GridManager _gridManager;
    [SerializeField] private GameObject _highlight;
    [SerializeField] private PathValidator _pathValidator;
    [SerializeField] private TowerData _towerToPlace;
    [SerializeField] private PlayerResources _playerResources;

    private FrostmaulInput _input;
    private Vector2Int _selectedCell = new Vector2Int(-1, -1);
    private bool _hasSelection;
    private readonly Dictionary<Vector2Int, TowerBase> _placedTowers
    = new Dictionary<Vector2Int, TowerBase>();


    public static event Action<Vector2Int> OnCellSelected;
    public static event Action OnSelectionCleared;
    public static event Action<Vector2Int> OnTowerCellTapped;


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
        if (TowerMenu.IsOpen) return;
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
        else if (_gridManager.GetCellState(cell) == CellState.Tower)
        {
            ClearSelection();
            OnTowerCellTapped?.Invoke(cell);
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
        OnCellSelected?.Invoke(cell);
    }

    private void PlaceTower(Vector2Int cell)
    {
        if (!_pathValidator.IsPlacementValid(cell)) return;
        if (_towerToPlace == null || _towerToPlace.Prefab == null) return;
        if (!_playerResources.TrySpend(_towerToPlace.Cost)) return;

        _gridManager.SetCellState(cell, CellState.Tower);
        GameObject go = Instantiate(_towerToPlace.Prefab, _gridManager.CellToWorld(cell), Quaternion.identity);
        TowerBase tower = go.GetComponent<TowerBase>();
        if (tower != null) _placedTowers[cell] = tower;
        ClearSelection();
    }


    private void ClearSelection()
    {
        _hasSelection = false;
        _selectedCell = new Vector2Int(-1, -1);
        _highlight.SetActive(false);
        OnSelectionCleared?.Invoke();
    }

    public void SetTowerData(TowerData data)
    {
        _towerToPlace = data;
    }

    public void PlaceCurrentSelection()
    {
        if (!_hasSelection) return;
        PlaceTower(_selectedCell);
    }

    public void CancelSelection()
    {
        ClearSelection();
    }

    public TowerBase GetTowerAt(Vector2Int cell)
    {
        _placedTowers.TryGetValue(cell, out TowerBase tower);
        return tower;
    }

    public void SellTower(Vector2Int cell)
    {
        if (!_placedTowers.TryGetValue(cell, out TowerBase tower)) return;

        int refund = tower.Data.Cost / 2;
        _placedTowers.Remove(cell);
        Destroy(tower.gameObject);
        _gridManager.SetCellState(cell, CellState.Empty);
        _playerResources.AddGold(refund);
    }
}
