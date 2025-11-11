using UnityEngine;
using UnityEngine.EventSystems;

public class DragAndZoom : MonoBehaviour
{
    [SerializeField] private float zoomSpeed = 0.1f; // Speed of zooming
    [SerializeField] private float minZoom = 0.5f;   // Minimum scale
    [SerializeField] private float maxZoom = 3.0f;   // Maximum scale

    private readonly Vector2 _dragLimitOffset = new(Screen.width * 0.75f, Screen.height * 0.75f);
    private Camera _mainCamera;
    private RectTransform _rectTransform;
    private Canvas _parentCanvas;
    private Vector2 _initialPosition;
    private Vector2 _minDragLimit;
    private Vector2 _maxDragLimit;

    private bool _isDragging = false;
    private Vector3 _dragOffset;

    private void Awake()
    {
        _mainCamera = Camera.main;
        _rectTransform = GetComponent<RectTransform>();
        _parentCanvas = GetComponentInParent<Canvas>();

        // Set the parent canvas to full screen
        SetCanvasToFullScreen();

        // Store the initial position of the RectTransform
        _initialPosition = _rectTransform.position;

        // Calculate drag limits based on the initial position and offset
        _minDragLimit = _initialPosition - _dragLimitOffset;
        _maxDragLimit = _initialPosition + _dragLimitOffset;
    }

    private void Update()
    {
        HandleDrag();
        HandleZoom();
    }

    private void HandleDrag()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!IsPointerOverUIElement())
            {
                _isDragging = true;
                _dragOffset = _rectTransform.position - GetMouseWorldPosition();
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            _isDragging = false;
        }

        if (!_isDragging) return;

        var newPosition = GetMouseWorldPosition() + _dragOffset;

        // Apply drag limits relative to the initial position
        newPosition.x = Mathf.Clamp(newPosition.x, _minDragLimit.x, _maxDragLimit.x);
        newPosition.y = Mathf.Clamp(newPosition.y, _minDragLimit.y, _maxDragLimit.y);

        _rectTransform.position = newPosition;
    }

    private Vector3 GetMouseWorldPosition()
    {
        var mousePosition = Input.mousePosition;
        mousePosition.z = _mainCamera.WorldToScreenPoint(_rectTransform.position).z;
        return _mainCamera.ScreenToWorldPoint(mousePosition);
    }

    private void HandleZoom()
    {
        var scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll == 0) return;

        var newScale = _rectTransform.localScale + Vector3.one * (scroll * zoomSpeed);
        newScale = Vector3.Max(newScale, Vector3.one * minZoom);
        newScale = Vector3.Min(newScale, Vector3.one * maxZoom);
        _rectTransform.localScale = newScale;
    }

    private bool IsPointerOverUIElement() => EventSystem.current.IsPointerOverGameObject();

    private void SetCanvasToFullScreen()
    {
        var canvasRectTransform = _parentCanvas.GetComponent<RectTransform>();
        canvasRectTransform.sizeDelta = new Vector2(Screen.width, Screen.height);
        canvasRectTransform.anchoredPosition = new Vector3(-Screen.width / 2f, 0);
    }
}