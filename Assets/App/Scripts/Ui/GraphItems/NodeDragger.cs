using Arcube;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(NodeObject))]
public class NodeDragger : MonoBehaviour, IDragHandler, IBeginDragHandler
{
    private NodeObject _nodeObject;
    private Canvas _canvas;
    private Vector2 _dragOffset;
    private RectTransform _rectTransform;
    private void Start()
    {
        _canvas = GetComponentInParent<Canvas>();
        _nodeObject = GetComponent<NodeObject>();
        _rectTransform = _nodeObject.GetComponent<RectTransform>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!_nodeObject.Editable) return;

        if (!_rectTransform || !_canvas) return;

        // Calculate the initial offset between the pointer and the object's position
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _canvas.transform as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out _dragOffset
        );
        _dragOffset -= _rectTransform.anchoredPosition;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!_nodeObject.Editable) return;

        if (!_rectTransform || !_canvas) return;

        // Adjust the position of the RectTransform based on the drag event
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _canvas.transform as RectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out var localPoint
        );

        var prevPosition = _rectTransform.anchoredPosition;

        _rectTransform.anchoredPosition = localPoint - _dragOffset;
        _nodeObject.Node.AnchoredPosition = new Vector2Simple(_rectTransform.anchoredPosition);

        var delta = _rectTransform.anchoredPosition - prevPosition;

        _nodeObject.MoveBranchNodes(delta);

        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            _nodeObject.MoveAllFollowingNodes(delta);
        }
    }
}