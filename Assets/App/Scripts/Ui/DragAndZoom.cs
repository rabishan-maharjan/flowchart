using UnityEngine;
using UnityEngine.EventSystems;

public class DragAndZoom : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float zoomSpeed = 0.1f; // Speed of zooming
    [SerializeField] private float minZoom = 0.5f;   // Minimum scale
    [SerializeField] private float maxZoom = 3.0f;   // Maximum scale

    private Vector3 _dragOffset;
    private RectTransform _rectTransform;
    private bool _isDragging;
    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
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

        if (Input.GetMouseButtonUp(0)) _isDragging = false;

        if (_isDragging) _rectTransform.position = GetMouseWorldPosition() + _dragOffset;
    }

    private Vector3 GetMouseWorldPosition()
    {
        var mousePosition = Input.mousePosition;
        mousePosition.z = mainCamera.WorldToScreenPoint(_rectTransform.position).z;
        return mainCamera.ScreenToWorldPoint(mousePosition);
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
}