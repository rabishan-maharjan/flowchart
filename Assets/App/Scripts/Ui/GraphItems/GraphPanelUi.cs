using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Arcube;
using Arcube.AssetManagement;
using Arcube.UiManagement;
using NaughtyAttributes;
using Newtonsoft.Json;
using UnityEngine;

public partial class GraphPanelUi : Ui
{
    private Canvas _canvas;
    private FlowChartManager _flowChartManager;
    public override Task Register()
    {
        _canvas = GetComponentInParent<Canvas>();
        _flowChartManager =AppManager.GetManager<FlowChartManager>();
        _flowChartManager.OnCodeStateChanged += async state =>
        {
            if (state == AppState.New)
            {
                Clear();
                
                var startNode = await AssetManager.Instantiate<NodeObject>("StartNode", container);
                startNode.transform.localPosition = Vector3.zero;
                
                var endNode = await AssetManager.Instantiate<NodeObject>("EndNode", container);
                var rt = (RectTransform)endNode.transform;
                rt.anchoredPosition = new Vector2(0, -rt.rect.height) - startNode.connectorMain.offset;
                
                Connect(startNode.connectorMain, endNode);
            }
        };
        return base.Register();
    }

    [SerializeField] private GameObject linePrefab;
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private Transform container;
    public void Connect(ConnectorObject connectorObject, NodeObject nodeObject)
    {
        if (nodeObject)
        {
            if (nodeObject is StartNodeObject)
            {
                Debug.LogWarning("Cannot connect to start");
                return;
            }
            
            if (nodeObject.connectorMain)
            {
                if (nodeObject.connectorMain.Connection == connectorObject.ParentNodeObject)
                {
                    Debug.LogWarning("cyclical");
                    nodeObject.connectorMain.Clear();
                }

                if (nodeObject.connectorMain == connectorObject)
                {
                    Debug.LogWarning("Self connection");
                    nodeObject.connectorMain.Clear();
                    return;
                }
            }

            if (nodeObject.connectorSecondary)
            {
                if (nodeObject.connectorSecondary.Connection == connectorObject.ParentNodeObject)
                {
                    Debug.LogWarning("cyclical");
                    nodeObject.connectorSecondary.Clear();
                }

                if (nodeObject.connectorSecondary == connectorObject)
                {
                    Debug.LogWarning("Self connection");
                    nodeObject.connectorSecondary.Clear();
                    return;
                }
            }

            if (nodeObject.OtherConnection is ConnectorObject oldConnector)
            {
                Debug.LogWarning("previously connected to something else");
                oldConnector.Clear();
            }
        }

        if (connectorObject.Line)
        {
            Debug.LogWarning("previously connected to line");
            connectorObject.Clear();
        }
        
        var line = DrawLine(connectorObject, nodeObject);
        line.SetConnection(connectorObject, nodeObject);
        connectorObject.Connection = nodeObject;
        connectorObject.Line = line;
        nodeObject.OtherConnection = connectorObject;
    }
    private void Clear()
    {
        foreach (var obj in GetComponentsInChildren<GraphObject>())
        {
            Destroy(obj.gameObject);
        }
    }

    public Action<GraphObject> OnSelected { get; set; }
    public static GraphObject Selected { get; set; }
    private void Update()
    {
        if (!Input.GetMouseButtonDown(1)) return;

        _ = CreateNode();
    }

    private async Task CreateNode()
    {
        // Get RectTransform of the container (the parent you instantiate into)
        var containerRect = container.GetComponent<RectTransform>();
        if (!containerRect) return;

        // Choose the correct camera for ScreenPointToLocalPointInRectangle
        var canvas = _canvas; // assume this is your Canvas component
        Camera cam = null;
        if (canvas.renderMode != RenderMode.ScreenSpaceOverlay)
            cam = canvas.worldCamera;

        // Convert screen point directly to local point in *container* coordinates
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(containerRect, Input.mousePosition, cam, out Vector2 localPoint))
            return;

        // Show node selection UI (await)
        var newNodePrefab = await UiManager.GetUi<NodeSelectUi>().SelectNode();
        if (!newNodePrefab) return;

        // Instantiate under container
        var nodeObject = Instantiate(newNodePrefab, container);
        var nodeRect = nodeObject.GetComponent<RectTransform>();
        if (!nodeRect) return;

        // Ensure scale is correct (prefab import sometimes has wrong scale)
        nodeRect.localScale = Vector3.one;

        // Set anchoredPosition to the local point we calculated
        nodeRect.anchoredPosition = localPoint;
    }

    [Button]
    public void GenerateCode()
    {
        foreach (var nodeObject in GetComponentsInChildren<NodeObject>())
        {
            if (!nodeObject.OtherConnection)
            {
                _flowChartManager.AddNode(nodeObject.Node, null, true);
            }
            else
            {
                var connector = (ConnectorObject)nodeObject.OtherConnection;
                _flowChartManager.AddNode(nodeObject.Node, connector.ParentNodeObject.Node,
                    connector.id == 0);
            }
        }
        
        Debug.Log(JsonConvert.SerializeObject(_flowChartManager.Functions, Formatting.Indented));
    }

    public async Task Decompile(Dictionary<string, Function> functions)
    {
        foreach (var nodeObject in GetComponentsInChildren<NodeObject>())
        {
            nodeObject.Delete(true);
        }

        var nodesObjects = new List<NodeObject>();
        foreach (var function in functions)
        {
            foreach (var node in function.Value.Nodes)
            {
                var obj = await AssetManager.Instantiate<NodeObject>(node.Name, container);
                obj.Node = node;
                var rt = (RectTransform)obj.transform;
                rt.anchoredPosition = node.AnchoredPosition.ToVector2();
                nodesObjects.Add(obj);
                Debug.Log(node.Name);
            }
        }
        
        //check for connections
        foreach (var function in functions)
        {
            foreach (var node in function.Value.Nodes)
            {
                var nodeObject = nodesObjects.Find(x => x.Node.ID == node.ID);
                if (node.NextMainNode != null)
                {
                    var nextNodeObject = nodesObjects.Find(x => x.Node.ID == node.NextMainNode);
                    if(nextNodeObject) Connect(nodeObject.connectorMain, nextNodeObject);
                }
                
                if (node.NextSecondaryNode != null)
                {
                    var nextNodeObject = nodesObjects.Find(x => x.Node.ID == node.NextSecondaryNode);
                    if(nextNodeObject) Connect(nodeObject.connectorSecondary, nextNodeObject);
                }
            }
        }
    }
}