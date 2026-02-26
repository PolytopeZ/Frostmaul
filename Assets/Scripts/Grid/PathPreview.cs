using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PathPreview : MonoBehaviour
{
    [SerializeField] private GridManager _gridManager;
    [SerializeField] private FlowFieldCalculator _flowField;
    [SerializeField] private GameObject _tileHighlightPrefab;
    [SerializeField] private TextMeshProUGUI _pathLengthLabel;
    [SerializeField] private Canvas _canvas;

    private FrostmaulInput _input;
    private GameObject[] _highlights;
    private int _poolSize;

    private void Awake()
    {
        _poolSize = _gridManager.Columns * _gridManager.Rows;
        _highlights = new GameObject[_poolSize];

        for (int i = 0; i < _poolSize; i++)
        {
            _highlights[i] = Instantiate(_tileHighlightPrefab);
            _highlights[i].SetActive(false);
        }

        _input = new FrostmaulInput();
    }

    private void OnEnable()
    {
        _input.Enable();
        _input.Gameplay.PreviewHold.performed += OnHoldPerformed;
        _input.Gameplay.PreviewHold.canceled += OnHoldCanceled;
    }

    private void OnDisable()
    {
        _input.Gameplay.PreviewHold.performed -= OnHoldPerformed;
        _input.Gameplay.PreviewHold.canceled -= OnHoldCanceled;
        _input.Disable();
    }

    private void OnHoldPerformed(InputAction.CallbackContext ctx)
    {
        Vector2 screenPos = _input.Gameplay.PointerPosition.ReadValue<Vector2>();
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, 0f));
        worldPos.z = 0f;

        if (_gridManager.WorldToCell(worldPos).x == Constants.InvalidCell) return;

        ShowPreview();
    }

    private void OnHoldCanceled(InputAction.CallbackContext ctx)
    {
        HidePreview();
    }

    private void ShowPreview()
    {
        List<Vector2Int> path = _flowField.GetPath();

        for (int i = 0; i < _poolSize; i++)
        {
            if (i < path.Count)
            {
                _highlights[i].transform.position = _gridManager.CellToWorld(path[i]);
                _highlights[i].SetActive(true);
            }
            else
            {
                _highlights[i].SetActive(false);
            }
        }

        _canvas.gameObject.SetActive(true);
        _pathLengthLabel.text = "Path: " + path.Count + " tiles";
    }

    private void HidePreview()
    {
        for (int i = 0; i < _poolSize; i++)
            _highlights[i].SetActive(false);

        _canvas.gameObject.SetActive(false);
        _pathLengthLabel.text = string.Empty;
    }
}
