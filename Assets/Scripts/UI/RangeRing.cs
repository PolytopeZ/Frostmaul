using UnityEngine;

public class RangeRing : MonoBehaviour
{
    [SerializeField] private GridManager _gridManager;

    private TowerPlacer _towerPlacer;
    [SerializeField][Range(16, 64)] private int _segments = 40;
    [SerializeField] private Color _color = new Color(1f, 1f, 0.3f, 0.55f);

    private LineRenderer _line;
    private Vector3 _cellWorldPos;

    private void Awake()
    {
        _towerPlacer = FindObjectOfType<TowerPlacer>();
        _line = GetComponent<LineRenderer>();
        _line.loop = true;
        _line.positionCount = _segments;
        _line.useWorldSpace = true;
        _line.widthMultiplier = 0.06f;
        _line.startColor = _color;
        _line.endColor = _color;
        _line.material = new Material(Shader.Find("Sprites/Default"));
        _line.sortingOrder = 5;
        _line.enabled = false;
    }

    private void OnEnable()
    {
        TowerPlacer.OnCellSelected += HandleCellSelected;
        TowerPlacer.OnSelectionCleared += HandleSelectionCleared;
        TowerPlacer.OnTowerCellTapped += HandleTowerCellTapped;
        TowerMenu.OnTowerPreview += HandleTowerPreview;
    }

    private void OnDisable()
    {
        TowerPlacer.OnCellSelected -= HandleCellSelected;
        TowerPlacer.OnSelectionCleared -= HandleSelectionCleared;
        TowerPlacer.OnTowerCellTapped -= HandleTowerCellTapped;
        TowerMenu.OnTowerPreview -= HandleTowerPreview;
    }

    // ── Event Handlers ────────────────────────────────────────────────────────

    private void HandleCellSelected(Vector2Int cell)
    {
        _cellWorldPos = _gridManager.CellToWorld(cell);
        _line.enabled = false; // hide until the player chooses a tower type
    }

    private void HandleTowerPreview(TowerData data)
    {
        // Finger pressed a tower button — show range at the selected cell
        ShowRing(_cellWorldPos, data.Range);
    }

    private void HandleTowerCellTapped(Vector2Int cell)
    {
        // Sell panel opened — show range of the existing tower
        TowerBase tower = _towerPlacer.GetTowerAt(cell);
        if (tower == null) return;
        ShowRing(_gridManager.CellToWorld(cell), tower.Data.Range);
    }

    private void HandleSelectionCleared()
    {
        _line.enabled = false;
    }

    // ── Drawing ───────────────────────────────────────────────────────────────

    private void ShowRing(Vector3 center, float radius)
    {
        center.z = -0.1f;
        _line.enabled = true;
        for (int i = 0; i < _segments; i++)
        {
            float angle = i * 2f * Mathf.PI / _segments;
            _line.SetPosition(i,
                center + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f) * radius);
        }
    }
}
