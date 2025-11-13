using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

[RequireComponent(typeof(RectTransform))]
public class UIDragAndMove : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Canvas _rootCanvas;
    private GraphicRaycaster _raycaster;

    private int _dragStartIndex;
    private Transform _dragStartParent;
    private LayoutGroup _layoutGroup;
    private static bool _isDragging;
    private static Transform _placeholder;

    private void Start()
    {
        _rootCanvas = GetComponentInParent<Canvas>();
        _raycaster = _rootCanvas.GetComponent<GraphicRaycaster>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        GetComponent<CanvasGroup>().interactable = false;
        _isDragging = true;

        _dragStartIndex = transform.GetSiblingIndex();
        _dragStartParent = transform.parent;
        _layoutGroup = _dragStartParent.GetComponent<LayoutGroup>();

        _placeholder = _dragStartParent.Find("placeholder");
        _placeholder.gameObject.SetActive(true);

        ((RectTransform)_placeholder).sizeDelta = ((RectTransform)transform).sizeDelta;
        _placeholder.SetSiblingIndex(_dragStartIndex);

        transform.SetParent(_rootCanvas.transform);
        if (TryGetComponent(out Image icon))
            icon.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Move object
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _rootCanvas.transform as RectTransform,
            Input.mousePosition,
            _rootCanvas.worldCamera,
            out var pos);

        transform.position = _rootCanvas.transform.TransformPoint(pos);

        // 🔍 Manually raycast to find UI elements under the pointer
        var results = new List<RaycastResult>();
        _raycaster.Raycast(eventData, results);

        foreach (var hit in results)
        {
            // Check if hit another draggable item in same parent
            var other = hit.gameObject.GetComponent<UIDragAndMove>();
            if (other && other.transform.parent == _dragStartParent && other.transform != transform)
            {
                int targetIndex = other.transform.GetSiblingIndex();
                _placeholder.SetSiblingIndex(targetIndex);
                break;
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        GetComponent<CanvasGroup>().interactable = true;
        _isDragging = false;

        if (TryGetComponent(out Image icon))
            icon.raycastTarget = true;

        // Move the item to placeholder's position
        var newIndex = _placeholder.GetSiblingIndex();
        transform.SetParent(_dragStartParent);
        transform.SetSiblingIndex(newIndex);

        _placeholder.gameObject.SetActive(false);
        LayoutRebuilder.ForceRebuildLayoutImmediate(_layoutGroup.GetComponent<RectTransform>());
    }
}
