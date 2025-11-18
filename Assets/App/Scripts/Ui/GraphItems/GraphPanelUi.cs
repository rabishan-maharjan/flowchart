using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Arcube;
using Arcube.AssetManagement;
using Arcube.UiManagement;
using NaughtyAttributes;
using UnityEngine;

public class GraphPanelUi : Ui
{
    private FlowChartManager _flowChartManager;
    [SerializeField] private Transform container;
    public override Task Register()
    {
        _flowChartManager = AppManager.GetManager<FlowChartManager>();
        _flowChartManager.OnCodeStateChanged += HandleCodeStateChanged;
        return base.Register();
    }

    private async void HandleCodeStateChanged(AppState state)
    {
        try
        {
            if (state != AppState.New) return;

            Clear();

            var startNode = await AssetManager.Instantiate<NodeObject>("StartNode", container);
            startNode.transform.localPosition = Vector3.zero;

            var endNode = await AssetManager.Instantiate<NodeObject>("EndNode", container);
            var rt = (RectTransform)endNode.transform;
            rt.anchoredPosition = new Vector2(0, -100);

            startNode.ConnectorObject.Connect(endNode);
            //Connect(startNode.connector, endNode);
        }
        catch(Exception e)
        {
            Log.AddException(e);
        }
    }
    
    private void Clear()
    {
        foreach (var obj in GetComponentsInChildren<GraphObject>())
        {
            Destroy(obj.gameObject);
        }
    }

    public static GraphObject Selected { get; set; }
    private void Update()
    {
        if (Input.GetKey(KeyCode.Delete) || Input.GetKey(KeyCode.Backspace))
        {
            Selected?.Delete(false);
        }

        if (Input.GetMouseButtonDown(1))
        {
            CreateNode();
        }
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
            var containerRect = container.GetComponent<RectTransform>();
            if (!containerRect) return;

            // Choose the correct camera for ScreenPointToLocalPointInRectangle
            var c = canvas; // assume this is your Canvas component
            Camera cam = null;
            if (c.renderMode != RenderMode.ScreenSpaceOverlay)
                cam = c.worldCamera;

            // Convert screen point directly to local point in *container* coordinates
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(containerRect, Input.mousePosition, cam, out var localPoint))
                return;

            // Show node selection UI (await)
            await nodeSelectUi.OpenAsync();
            var newNodePrefab = nodeSelectUi.Selected;
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
        catch (Exception e)
        {
            Log.AddException(e);
        }
    }

    [Button]
    public void GenerateCode()
    {
        _flowChartManager.ClearNodes();
        
        foreach (var nodeObject in GetComponentsInChildren<NodeObject>())
        {
            nodeObject.GenerateCode(_flowChartManager);
        }
        
        //Debug.Log(JsonConvert.SerializeObject(_flowChartManager.Functions, Formatting.Indented));
    }

    public async Task GenerateFlowChart(Dictionary<string, Function> functions)
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
            }
        }
        
        foreach (var function in functions)
        {
            foreach (var node in function.Value.Nodes)
            {
                var nodeObject = nodesObjects.Find(x => x.Node.ID == node.ID);
                if (node.NextNode != null)
                {
                    var nextNodeObject = nodesObjects.Find(x => x.Node.ID == node.NextNode);
                    if(nextNodeObject) nodeObject.ConnectorObject.Connect(nextNodeObject);
                }

                if (node is LogicCommand logicCommand)
                {
                    if (logicCommand.NodeTrue != null)
                    {
                        var nextNodeObject = nodesObjects.Find(x => x.Node.ID == logicCommand.NodeTrue);
                        if (nextNodeObject)
                        {
                            ((LogicNodeObject)nodeObject).connectorTrue.Connect(nextNodeObject);
                        }
                        else
                        {
                            Debug.Log($"Next node not found {logicCommand.NodeTrue}");
                        }
                    }
                    
                    if (logicCommand.NodeFalse != null)
                    {
                        var nextNodeObject = nodesObjects.Find(x => x.Node.ID == logicCommand.NodeFalse);
                        if (nextNodeObject)
                        {
                            ((LogicNodeObject)nodeObject).connectorFalse.Connect(nextNodeObject);
                        }
                        else
                        {
                            Debug.Log($"Next node not found {logicCommand.NodeFalse}");
                        }
                    }
                }
                else if (node is LoopCommand loopCommand)
                {
                    if (loopCommand.NodeLoop != null)
                    {
                        var nextNodeObject = nodesObjects.Find(x => x.Node.ID == loopCommand.NodeLoop);
                        if (nextNodeObject)
                        {
                            ((LoopNodeObject)nodeObject).ConnectorLoopObject.Connect(nextNodeObject);
                        }
                        else
                        {
                            Debug.Log($"Next node not found {loopCommand.NodeLoop}");
                        }
                    }
                }
            }
        }
    }
}