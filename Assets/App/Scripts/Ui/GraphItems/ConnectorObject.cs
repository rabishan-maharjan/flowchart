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

    //this is reference to the line drawer of the connector object that is connected to the branch connector
    public DynamicLineDrawer CloseLoopLineDrawer { get; set; }
    public void Connect(NodeObject nodeObject)
    {
        if (NextNodeObject)
        {
            NextNodeObject.PrevConnectorObject = null;
        }
        
        var branchConnector = GetBranchConnector();
        branchConnector?.CloseLoopLineDrawer?.Clear();

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
        
        Debug.Log("Adding line between " + name + " and " + nodeObject.name, gameObject);
        _ = lineDrawer.Set((RectTransform)nodeObject.transform);

        CreateBranchLoop(branchConnector);
    }

    private ConnectorObject GetBranchConnector()
    {
        var prevConnectorObject = this;
        ConnectorObject branchConnector = null;
        while (prevConnectorObject)
        {
            if (prevConnectorObject.branchNode)
            {
                branchConnector = prevConnectorObject.ParentNodeObject is LogicNodeObject ? prevConnectorObject.ParentNodeObject.ConnectorObject : prevConnectorObject;
                break;
            }

            prevConnectorObject = prevConnectorObject.ParentNodeObject.PrevConnectorObject;
        }

        return branchConnector;
    }
    
    private void CreateBranchLoop(ConnectorObject branchConnector)
    {
        if (!branchConnector) return;
        
        var lastNodeObject = NextNodeObject;
        while (lastNodeObject)
        {
            if (!lastNodeObject.ConnectorObject.NextNodeObject)
            {
                break;
            }

            lastNodeObject = lastNodeObject.ConnectorObject.NextNodeObject;
        }
        
        lastNodeObject?.CloseNode(branchConnector);
    }
    
    public void Clear()
    {
        if (!NextNodeObject) return;
        NextNodeObject.PrevConnectorObject = null;

        if (!NextNodeObject.ConnectorObject.NextNodeObject)
        {
            if (NextNodeObject.ConnectorObject.CloseLoopLineDrawer)
            {
                Destroy(NextNodeObject.ConnectorObject.CloseLoopLineDrawer);
            }
        }
        
        if (TryGetComponent(out DynamicLineDrawer lineDrawer))
        {
            Destroy(lineDrawer);
        }
            
        NextNodeObject = null;
    }
}