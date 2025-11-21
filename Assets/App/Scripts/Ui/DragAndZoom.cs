using NaughtyAttributes;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragAndZoom : MonoBehaviour
{
    [SerializeField] private float zoomSpeed = 0.1f; // Speed of zooming
    [SerializeField] private float minZoom = 0.5f;   // Minimum scale
    [SerializeField] private float maxZoom = 3.0f;   // Maximum scale

    private Camera _mainCamera;
    private RectTransform _rectTransform;
    private Canvas _parentCanvas;

    private bool _isDragging;
    private Vector3 _dragOffset;
    private void Awake()
    {
        _mainCamera = Camera.main;
        _rectTransform = GetComponent<RectTransform>();
        _parentCanvas = GetComponentInParent<Canvas>();

        // Set the parent canvas to full screen
        SetCanvasToFullScreen();
    }

    private void Update()
    {
        HandleDrag();
        HandleZoom();
        HandleShortcuts();
    }

    private void HandleShortcuts()
    {
        if (!Input.GetKey(KeyCode.LeftControl) && !Input.GetKey(KeyCode.RightControl)) return;
        if (!Input.GetKeyDown(KeyCode.F)) return;
        ResetPosition();
    }

    [Button]
    private void ResetPosition()
    {
        var startNode = GetComponentInChildren<StartNodeObject>();
        var startNodeRect = startNode.GetComponent<RectTransform>();
        var containerRect = _rectTransform;

        var canvas = containerRect.GetComponentInParent<Canvas>();
        var cam = canvas.worldCamera;

        // Distance from camera to the canvas plane
        float dist = Mathf.Abs(canvas.transform.position.z - cam.transform.position.z);

        // 1. Convert screen center to world using the correct Z distance
        Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, dist);
        Vector3 worldCenter = cam.ScreenToWorldPoint(screenCenter);

        // 2. Find StartNode’s world position
        Vector3 nodeWorldPos = startNodeRect.position;

        // 3. Compute world offset
        Vector3 offset = worldCenter - nodeWorldPos;

        // 4. Apply offset to container
        containerRect.position += offset;
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
        canvasRectTransform.anchoredPosition = new Vector3(0, 0);
    }
}