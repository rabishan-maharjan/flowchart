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
    [SerializeField] public bool branchNode = false;
    [SerializeField] private NodeType nodeType = NodeType.Flow;
    [field: SerializeField] public NodeObject ParentNodeObject { get; set; }
    public NodeObject NextNodeObject { get; private set; }

    [field: SerializeField] public Vector3 NextNodeOffset { get; set; } = new Vector3(0, -100, 0);

    protected override void Reset()
    {
        ParentNodeObject = GetComponentInParent<NodeObject>();
        base.Reset();
    }

    public RectTransform RectTransform { get; private set; }
    private void Start()
    {
        RectTransform = GetComponent<RectTransform>();
        GetComponent<ButtonImage>().OnClick.AddListener(Select);
    }
    
    private async void CreateNode()
    {
        try
        {
            var nodeSelectUi = UiManager.GetUi<NodeSelectUi>();
            if (nodeSelectUi.State is UiState.Opened or UiState.Opening)
            {
                nodeSelectUi.Close();
                return;
            }
        
            // Get RectTransform of the container (the parent you instantiate into)
            var containerRect = ParentNodeObject.transform.parent.GetComponent<RectTransform>();
            if (!containerRect) return;

            // Choose the correct camera for ScreenPointToLocalPointInRectangle
            // Show node selection UI (await)
            await nodeSelectUi.OpenAsync();
            var newNodePrefab = nodeSelectUi.Selected;
            if (!newNodePrefab) return;

            // Instantiate under container
            var nodeObject = Instantiate(newNodePrefab, containerRect);
            var nodeRect = nodeObject.GetComponent<RectTransform>();
            if (!nodeRect) return;

            // Ensure scale is correct (prefab import sometimes has wrong scale)
            nodeRect.localScale = Vector3.one;

            // Set anchoredPosition to the local point we calculated
            nodeObject.transform.position = transform.position + NextNodeOffset;

            if (!branchNode && NextNodeObject)
            {
                nodeObject.ConnectorObject.Connect(NextNodeObject);
                nodeObject.MoveAllFollowingNodes(NextNodeOffset);
            }
            
            Connect(nodeObject);
        }
        catch (Exception e)
        {
            Log.AddException(e);
        }
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

            nodeObject.Connect(this);
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

    private void Update()
    {
        if (Hovered && Input.GetMouseButtonDown(1))
        {
            CreateNode();
        }
    }
}