using System.Collections;
using UnityEngine;
using Arcube;

public class NodeObject : GraphObject
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
    protected FlowChartManager FlowChartManager;
    protected override void Awake()
    {
        base.Awake();
        _rectTransform = GetComponent<RectTransform>();
        FlowChartManager = AppManager.GetManager<FlowChartManager>();
    }

    protected virtual IEnumerator Start()
    {
        yield return null;
        yield return null;

        Image.color = GraphSettings.Instance.colors[Node.Name];

        Node.AnchoredPosition = new Vector2Simple(_rectTransform.anchoredPosition);
    }
    
    public void MoveAllFollowingNodes(Vector3 delta)
    {
        var next = ConnectorObject?.NextNodeObject;
        while (next)
        {
            next.Move(delta);
            next.MoveBranchNodes(delta);
            next = next.ConnectorObject?.NextNodeObject;
        }
    }
    
    public virtual void MoveBranchNodes(Vector2 delta)
    {
    }

    public void Move(Vector2 delta)
    {
        _rectTransform.anchoredPosition += delta;
        Node.AnchoredPosition = new Vector2Simple(_rectTransform.anchoredPosition);
    }
    
    protected override void Select()
    {
        if (!Editable) return;

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
        if (!Editable) return false;

        if (connectorObject.NextNodeObject == this)
        {
            //no need to connect
            Debug.Log("Already connected to this node");
            return false;
        }

        if (this is StartNodeObject)
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

    public void Connect(ConnectorObject connectorObject)
    {
        _node.BranchID = connectorObject.branchNode
            ? connectorObject.ParentNodeObject.Node.ID
            : connectorObject.ParentNodeObject._node.BranchID;
        //Debug.Log($"BranchID: {_node.BranchID} {name}", gameObject);
    }

    public void CloseNode(ConnectorObject newBranchConnectorObject)
    {
        if (this is EndNodeObject) return;

        var lineDrawer = ConnectorObject.GetComponent<DynamicLineDrawer>();
        if (!lineDrawer)
        {
            lineDrawer = ConnectorObject.gameObject.AddComponent<DynamicLineDrawer>();
        }

        _ = lineDrawer.Set(newBranchConnectorObject.transform.GetChild(0) as RectTransform, false);
    }

    public bool IsVariableUsed(string variable) => Node.IsVariableUsed(variable);

    public override void Delete(bool force)
    {
        if (!force && GraphPanelUi.Selected != this) return;
        GraphPanelUi.Selected = null;

        //delete lines from previous connector
        PrevConnectorObject?.Clear();

        ConnectorObject?.Clear();
        base.Delete(force);
    }

    public virtual void GenerateCode()
    {
        if (ConnectorObject && ConnectorObject.NextNodeObject)
        {
            var nextNode = ConnectorObject.NextNodeObject.Node;
            if (nextNode != null)
            {
                Node.NextNode = nextNode.ID;
            }
        }

        FlowChartManager.AddNode(Node);
    }
}