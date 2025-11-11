using Arcube.UiManagement;
using UnityEngine;

public enum NodeType
{
    Input,
    Flow,
}

public class ConnectorObject : GraphObject
{
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
        Debug.Log($"Connecting {name} with {nodeObject.name}");
        
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
    }
    
    public void Clear()
    {
        NextNodeObject = null;
        Debug.Log($"Clearing {name} child of {ParentNodeObject.name}", gameObject);
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