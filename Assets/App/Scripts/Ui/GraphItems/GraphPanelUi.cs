using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Arcube;
using Arcube.AssetManagement;
using Arcube.UiManagement;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

public class GraphPanelUi : Ui
{
    public static bool IsDirty { get; set; }
    private FlowChartManager _flowChartManager;
    [SerializeField] private Transform container;
    public override Task Register()
    {
        _flowChartManager = AppManager.GetManager<FlowChartManager>();
        _flowChartManager.OnProjectStateChanged += HandleProjectStateChanged;
        _flowChartManager.OnCompileStateChanged += HandleCompileStateChanged;
        return base.Register();
    }

    private async void HandleProjectStateChanged(ProjectState state, string projectName)
    {
        try
        {
            switch (state)
            {
                case ProjectState.New:
                {
                    await New();
                    break;
                }
                case ProjectState.Load:
                    await GenerateFlowChart(_flowChartManager.Functions);
                    break;
            }
        }
        catch(Exception e)
        {
            Log.AddException(e);
        }
    }
    
    private void HandleCompileStateChanged(CompileState compileState)
    {
        if (compileState != CompileState.Compile) return;
        Selected = null;
        GenerateCode();
    }

    private async Task New()
    {
        Clear();
        _flowChartManager.ClearNodes();

        var startNode = await AssetManager.Instantiate<NodeObject>("StartNode", container);
        startNode.transform.localPosition = Vector3.zero;
        
        var endNode = await AssetManager.Instantiate<NodeObject>("EndNode", container);
        var rt = (RectTransform)endNode.transform;
        rt.anchoredPosition = new Vector2(0, -100);

        startNode.ConnectorObject.Connect(endNode);
        //Connect(startNode.connector, endNode);
    }
    
    private void Clear()
    {
        foreach (var obj in GetComponentsInChildren<GraphObject>())
        {
            Destroy(obj.gameObject);
        }
    }

    public bool RemoveVariable(Variable variable)
    {
        foreach (var nodeObject in GetComponentsInChildren<NodeObject>())
        {
            if (nodeObject.IsVariableUsed(variable.ID))
            {
                //maybe quick highlight
                MessageUi.Show($"Variable {variable.Name} is used in {nodeObject.Node.Name}");
                return false;
            }
        }
        
        AppManager.GetManager<FlowChartManager>().RemoveVariable(variable);
        return true;
    }

    public static GraphObject Selected { get; set; }
    private void Update()
    {
        if (Input.GetKey(KeyCode.Delete) || Input.GetKey(KeyCode.Backspace))
        {
            Selected?.Delete(false);
        }
    }

    [Button]
    private void GenerateCode()
    {
        _flowChartManager.ClearNodes();
        
        foreach (var nodeObject in GetComponentsInChildren<NodeObject>())
        {
            nodeObject.GenerateCode();
        }
        
        //Debug.Log(JsonConvert.SerializeObject(_flowChartManager.Functions, Formatting.Indented));
    }

    private async Task GenerateFlowChart(Dictionary<string, Function> functions)
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
                //Debug.Log($"Instantiating {node.Name}");
                var obj = await AssetManager.Instantiate<NodeObject>(node.Name, container);
                obj.Node = node;
                if (node is Command command)
                {
                    obj.Text = command.GetDescription();
                    LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)obj.transform);
                }
                
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
                else if (node is ForLoopCommand forLoopCommand)
                {
                    if (forLoopCommand.NodeLoop != null)
                    {
                        var nextNodeObject = nodesObjects.Find(x => x.Node.ID == forLoopCommand.NodeLoop);
                        if (nextNodeObject)
                        {
                            ((ForLoopNodeObject)nodeObject).ConnectorLoopObject.Connect(nextNodeObject);
                        }
                        else
                        {
                            Debug.Log($"Next node not found {forLoopCommand.NodeLoop}");
                        }
                    }
                }
                else if (node is WhileLoopCommand whileLoopCommand)
                {
                    if (whileLoopCommand.NodeLoop != null)
                    {
                        var nextNodeObject = nodesObjects.Find(x => x.Node.ID == whileLoopCommand.NodeLoop);
                        if (nextNodeObject)
                        {
                            ((WhileLoopNodeObject)nodeObject).ConnectorLoopObject.Connect(nextNodeObject);
                        }
                        else
                        {
                            Debug.Log($"Next node not found {whileLoopCommand.NodeLoop}");
                        }
                    }
                }
            }
        }
    }
}