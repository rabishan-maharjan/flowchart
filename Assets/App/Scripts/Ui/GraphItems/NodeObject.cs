using System.Collections;
using UnityEngine;
using Arcube.UiManagement;
using UnityEngine.EventSystems;
using Arcube;

public class GraphObject : PanelItem, IPointerClickHandler
{
    protected GraphPanelUi GraphPanelUi;
    protected override void Awake()
    {
        base.Awake();
        GraphPanelUi = GetComponentInParent<GraphPanelUi>();
    }
    
    public virtual void OnPointerClick(PointerEventData eventData)
    {
        Select();
    }

    protected virtual void Select()
    {
        GraphPanelUi.Selected = this;
    }

    public virtual void Delete(bool force)
    {
        Destroy(gameObject);
    }
}

public class NodeObject : GraphObject, IDragHandler, IBeginDragHandler
{
    public GraphObject OtherConnection { get; set; }
    protected Node _node;
    public virtual Node Node { get => _node;
        set => _node = value;
    }
    
    public ConnectorObject connectorMain;
    public ConnectorObject connectorSecondary;
    
    protected override void Reset()
    {
        connectorMain = GetComponentInChildren<ConnectorObject>();
        base.Reset();
    }

    private RectTransform _rectTransform;
    private Canvas _canvas;
    private Vector2 _dragOffset;
    
    protected override void Awake()
    {
        base.Awake();
        _rectTransform = GetComponent<RectTransform>();
        _canvas = GetComponentInParent<Canvas>();
    }

    private IEnumerator Start()
    {
        yield return null;
        yield return null;
        
        Node.AnchoredPosition = new Vector2Simple(_rectTransform.anchoredPosition);
    }

    public void SetText(string text) => Text = text;

    public void OnBeginDrag(PointerEventData eventData)
    {
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

    public virtual void OnDrag(PointerEventData eventData)
    {
        if (!_rectTransform || !_canvas) return;

        // Adjust the position of the RectTransform based on the drag event
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _canvas.transform as RectTransform, 
            eventData.position, 
            eventData.pressEventCamera, 
            out var localPoint
        );

        _rectTransform.anchoredPosition = localPoint - _dragOffset;

        if (OtherConnection is ConnectorObject connectorObject)
        {
            GraphPanelUi.UpdateLine(connectorObject.Line, connectorObject, this);
        }

        if (connectorMain && connectorMain.Line)
        {
            GraphPanelUi.UpdateLine(connectorMain.Line, connectorMain, (NodeObject)connectorMain.Connection);
        }
        if (connectorSecondary && connectorSecondary.Line)
        {
            GraphPanelUi.UpdateLine(connectorSecondary.Line, connectorSecondary, (NodeObject)connectorSecondary.Connection);
        }
        
        Node.AnchoredPosition = new Vector2Simple(_rectTransform.anchoredPosition);
    }

    protected override void Select()
    {
        if (GraphPanelUi.Selected is ConnectorObject connectorObject)
        {
            GraphPanelUi.Connect(connectorObject, this);
        }
        
        base.Select();
    }
    
    public override void Delete(bool force)
    {
        if(!force && GraphPanelUi.Selected != this) return;
        GraphPanelUi.Selected = null;
        
        connectorMain?.Clear();
        base.Delete(force);
    }
}