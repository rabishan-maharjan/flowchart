using Arcube.UiManagement;
using UnityEngine;

public enum NodeType
{
    Input,
    Flow,
}

public class ConnectorObject : GraphObject
{
    [SerializeField] private bool branchNode = false;
    [SerializeField] private NodeType nodeType = NodeType.Flow;
    [field: SerializeField] public NodeObject ParentNodeObject { get; set; }
    public NodeObject NextNodeObject { get; private set; }
    
    protected override void Reset()
    {
        ParentNodeObject = GetComponentInParent<NodeObject>();
        base.Reset();
    }

    private void Start()
    {
        GetComponent<ButtonImage>().OnClick.AddListener(Select);
    }

    public override void Delete(bool force) => Clear();

    public void Connect(NodeObject nodeObject)
    {
        if (NextNodeObject)
        {
            NextNodeObject.PrevConnectorObject = null;
        }

        if (nodeObject.PrevConnectorObject)
        {
            nodeObject.PrevConnectorObject.Clear();
        }
        
        NextNodeObject = nodeObject;
        nodeObject.PrevConnectorObject = this;

        if(!TryGetComponent(out DynamicLineDrawer lineDrawer))
        {
            lineDrawer = gameObject.AddComponent<DynamicLineDrawer>();
        } 
        
        _ = lineDrawer.Set((RectTransform)nodeObject.transform);

        var branchConnector = GetBranchConnector();
        if (branchConnector)
        {
            NextNodeObject.CloseNode(branchConnector);
        }
    }

    private ConnectorObject GetBranchConnector()
    {
        var prevConnectorObject = this;
        while (prevConnectorObject)
        {
            if (prevConnectorObject.branchNode)
            {
                return prevConnectorObject.ParentNodeObject is LogicNodeObject ? prevConnectorObject.ParentNodeObject.ConnectorObject : prevConnectorObject;
            }

            prevConnectorObject = prevConnectorObject.ParentNodeObject.PrevConnectorObject;
        }

        return null;
    }
    
    public void Clear()
    {
        NextNodeObject = null;
        if (NextNodeObject)
        {
            NextNodeObject.PrevConnectorObject = null;
        }
        
        if (TryGetComponent(out DynamicLineDrawer lineDrawer))
        {
            Destroy(lineDrawer);
        }
    }
}