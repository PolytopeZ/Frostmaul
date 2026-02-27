using UnityEngine;
using UnityEngine.InputSystem;

public class CameraScroller : MonoBehaviour
{
    [SerializeField] private GridManager _gridManager;
    [SerializeField] private float _dragThresholdPixels = 20f;

    private Camera _camera;
    private FrostmaulInput _input;
    private bool _pressing;
    private bool _didDragThisPress;
    private float _accumulatedPixels;
    private float _minY;
    private float _maxY;

    public static bool IsDragging { get; private set; }

    // ── Unity Lifecycle ───────────────────────────────────────────────────────

    private void Awake()
    {
        _camera = GetComponent<Camera>();
        _input = new FrostmaulInput();
    }

    private void Start()
    {
        RefreshBounds();
    }

    private void OnEnable()
    {
        _input.Enable();
        _input.Gameplay.Press.started += OnPressStarted;
        _input.Gameplay.Press.canceled += OnPressCanceled;
        _input.Gameplay.DragDelta.performed += OnDragDelta;
    }

    private void OnDisable()
    {
        _input.Gameplay.Press.started -= OnPressStarted;
        _input.Gameplay.Press.canceled -= OnPressCanceled;
        _input.Gameplay.DragDelta.performed -= OnDragDelta;
        _input.Disable();
    }

    // ── Input Callbacks ───────────────────────────────────────────────────────

    private void OnPressStarted(InputAction.CallbackContext ctx)
    {
        IsDragging = false;
        _pressing = true;
        _didDragThisPress = false;
        _accumulatedPixels = 0f;
    }

    private void OnPressCanceled(InputAction.CallbackContext ctx)
    {
        _pressing = false;
    }

    private void OnDragDelta(InputAction.CallbackContext ctx)
    {
        if (!_pressing) return;

        Vector2 delta = ctx.ReadValue<Vector2>();

        if (!_didDragThisPress)
        {
            _accumulatedPixels += Mathf.Abs(delta.y);
            if (_accumulatedPixels >= _dragThresholdPixels)
            {
                _didDragThisPress = true;
                IsDragging = true;
            }
        }

        if (!_didDragThisPress) return;

        float worldDelta = delta.y * (_camera.orthographicSize * 2f) / Screen.height;

        Vector3 pos = _camera.transform.position;
        pos.y = Mathf.Clamp(pos.y + worldDelta, _minY, _maxY);
        _camera.transform.position = pos;
    }

    // ── Bounds ────────────────────────────────────────────────────────────────

    private void RefreshBounds()
    {
        float gridTopY = _gridManager.transform.position.y;
        float gridBottomY = gridTopY - (_gridManager.Rows * Constants.CellSize);
        float halfHeight = _camera.orthographicSize;

        _maxY = gridTopY - halfHeight;
        _minY = gridBottomY + halfHeight;

        if (_maxY < _minY)
        {
            float center = (gridTopY + gridBottomY) * 0.5f;
            _maxY = center;
            _minY = center;
        }
    }
}
