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
        var startNode = transform.GetComponentInChildren<StartNodeObject>();
        var startNodeRectTransform = startNode.GetComponent<RectTransform>();
        var pContainerRectTransform = _rectTransform;

        // Calculate the offset to position StartNode at the top center
        var newPosition = new Vector2(
            -startNodeRectTransform.anchoredPosition.x,
            -startNodeRectTransform.anchoredPosition.y + Screen.height / 2f - 100
        );

        // Apply the new position to p_container's RectTransform
        pContainerRectTransform.anchoredPosition = newPosition;
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