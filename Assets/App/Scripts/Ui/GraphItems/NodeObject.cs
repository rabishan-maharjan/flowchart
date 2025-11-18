using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using Arcube;

public class NodeObject : GraphObject, IDragHandler, IBeginDragHandler
{
    protected Node _node;
    public virtual Node Node
    {
        get => _node;
        set => _node = value;
    }
    public ConnectorObject PrevConnectorObject { get; set; }
    [field: SerializeField] public ConnectorObject ConnectorObject { get; private set; }
    protected override void Reset()
    {
        ConnectorObject = GetComponentInChildren<ConnectorObject>();
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

        Image.color = GraphSettings.Instance.colors[Node.Name];

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
        Node.AnchoredPosition = new Vector2Simple(_rectTransform.anchoredPosition);
    }

    protected override void Select()
    {
        if (GraphPanelUi.Selected is ConnectorObject connectorObject)
        {
            if (CanConnect(connectorObject))
            {
                connectorObject.Connect(this);
            }
        }

        base.Select();
    }
    
    //connection from connector object to node
    protected virtual bool CanConnect(ConnectorObject connectorObject)
    {
        if(connectorObject.NextNodeObject == this)
        {
            //no need to connect
            Debug.Log("Already connected to this node");
            return false;
        }
        
        if(this is StartNodeObject)
        {
            Debug.Log("Cannot connect to start node");
            return false;
        }
        
        //end node doesn't have a connector
        if (!ConnectorObject) return true;
        
        if (ConnectorObject.NextNodeObject == connectorObject.ParentNodeObject)
        {
            Debug.LogWarning("cyclical");
            return false;
        }
            
        if (ConnectorObject == connectorObject)
        {
            Debug.LogWarning("Self connection");
            return false;
        }

        return true;
    }

    public void CloseNode(ConnectorObject newBranchConnectorObject)
    {
        if(this is EndNodeObject) return;
        
        newBranchConnectorObject.CloseLoopLineDrawer = ConnectorObject.GetComponent<DynamicLineDrawer>();
        if (!newBranchConnectorObject.CloseLoopLineDrawer)
        {
            newBranchConnectorObject.CloseLoopLineDrawer = ConnectorObject.gameObject.AddComponent<DynamicLineDrawer>();
        }
        
        _ = newBranchConnectorObject.CloseLoopLineDrawer.Set(newBranchConnectorObject.transform.GetChild(0) as RectTransform, false);
    }

    public override void Delete(bool force)
    {
        if (!force && GraphPanelUi.Selected != this) return;
        GraphPanelUi.Selected = null;

        //delete lines from previous connector
        PrevConnectorObject?.Clear();
        
        ConnectorObject?.Clear();
        base.Delete(force);
    }

    public virtual void GenerateCode(FlowChartManager flowChartManager)
    {
        if (ConnectorObject && ConnectorObject.NextNodeObject)
        {
            var nextNode = ConnectorObject.NextNodeObject.Node;
            if (nextNode != null)
            {
                Node.NextNode = nextNode.ID;
            }
        }
        
        flowChartManager.AddNode(Node);
    }
}