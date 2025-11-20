using System;
using System.Threading.Tasks;
using Arcube;
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
    public async void Connect(NodeObject nodeObject)
    {
        try
        {
            if (NextNodeObject)
            {
                NextNodeObject.PrevConnectorObject = null;
            }

            var branchConnector = GetBranchConnector();
            if (branchConnector) ClearBranchLoop(branchConnector.NextNodeObject);

            if (nodeObject.PrevConnectorObject)
            {
                nodeObject.PrevConnectorObject.Clear();
            }

            NextNodeObject = nodeObject;
            nodeObject.PrevConnectorObject = this;
        
            await Task.Yield();
            await Task.Yield();
        
            if (!TryGetComponent(out DynamicLineDrawer lineDrawer))
            {
                //Debug.Log("Adding new line drawer");
                lineDrawer = gameObject.AddComponent<DynamicLineDrawer>();
            }
        
            //Debug.Log($"Adding line between {name} and {nodeObject.name} {Time.time}", gameObject);
            _ = lineDrawer.Set((RectTransform)nodeObject.transform);

            CreateBranchLoop(branchConnector);
        }
        catch (Exception e)
        {
            Log.AddException(e);
        }
    }

    private ConnectorObject GetBranchConnector()
    {
        var prevConnectorObject = this;
        while (prevConnectorObject)
        {
            if (prevConnectorObject.branchNode)
            {
                return prevConnectorObject;
            }

            prevConnectorObject = prevConnectorObject.ParentNodeObject.PrevConnectorObject;
        }

        return null;
    }

    private void CreateBranchLoop(ConnectorObject branchConnector)
    {
        if (!branchConnector) return;

        var lastNodeObject = branchConnector.NextNodeObject;
        while (lastNodeObject)
        {
            if (!lastNodeObject.ConnectorObject.NextNodeObject)
            {
                break;
            }

            lastNodeObject = lastNodeObject.ConnectorObject.NextNodeObject;
        }

        if (branchConnector.ParentNodeObject is LogicNodeObject logicNodeObject)
        {
            lastNodeObject?.CloseNode(logicNodeObject.ConnectorObject);    
        }
        else if(branchConnector.ParentNodeObject is ForLoopNodeObject loopNode)
        {
            lastNodeObject?.CloseNode(loopNode.ConnectorLoopObject);
        }
        else if(branchConnector.ParentNodeObject is WhileLoopNodeObject whileLoopNode)
        {
            lastNodeObject?.CloseNode(whileLoopNode.ConnectorLoopObject);
        }
    }

    private void ClearBranchLoop(NodeObject nodeObject)
    {
        var nextNodeObject = nodeObject;
        while (nextNodeObject)
        {
            if (!nextNodeObject.ConnectorObject) break;
            
            if (!nextNodeObject.ConnectorObject.NextNodeObject &&  nextNodeObject.ConnectorObject.TryGetComponent<DynamicLineDrawer>(out var lineDrawer))
            {
                Debug.LogWarning($"Destroying line drawer {lineDrawer.name} {Time.time}", lineDrawer.gameObject);
                Destroy(lineDrawer);
            }

            nextNodeObject = nextNodeObject.ConnectorObject.NextNodeObject;
        }
    }

    public async void Clear()
    {
        try
        {
            if (!NextNodeObject) return;
            NextNodeObject.PrevConnectorObject = null;

            ClearBranchLoop(NextNodeObject);

            if (TryGetComponent(out DynamicLineDrawer lineDrawer))
            {
                Destroy(lineDrawer);
            }

            NextNodeObject = null;

            await Task.Yield();
            await Task.Yield();
        
            CreateBranchLoop(GetBranchConnector());

            GraphPanelUi.Selected = null;
        }
        catch (Exception e)
        {
            Log.AddException(e);
        }
    }
}