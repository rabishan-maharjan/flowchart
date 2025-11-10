using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Arcube;

public enum AppState
{
    New,
    Load,
    Run
}

public class FlowChartManager : ManagerBase
{
    private string _activeFunction = "Main";
    public Dictionary<string, Function> Functions { get; set; } = new();
    public List<Variable> ActiveVariables => Functions[_activeFunction].Variables;
    public List<Node> ActiveNodes => Functions[_activeFunction].Nodes;
    public override Task<bool> Initialize()
    {
        New();
        return base.Initialize();
    }

    public void Clear()
    {
        Functions.Clear();
    }

    public event Action<AppState> OnCodeStateChanged;
    public void New()
    {
        Functions.Clear();
        Functions.Add(_activeFunction, new Function());
        
        OnCodeStateChanged?.Invoke(AppState.New);
    }
    
    public void SetActiveFunction(string function)
    {
        _activeFunction = function;
    }
    
    public void AddFunction(string function)
    {
        Functions.Add(function, new Function());
    }
    
    public void RemoveFunction(string function)
    {
        Functions.Remove(function);
    }

    public void AddVariable(Variable variable)
    {
        var variables = Functions[_activeFunction].Variables;
        if(variables.Contains(variable)) return;
        variables.Add(variable);
    }
    
    public void RemoveVariable(Variable variable)
    {
        var variables = Functions[_activeFunction].Variables;
        variables.Remove(variable);
    }
    
    public void Compile()
    {
        Functions["Main"].Execute();
    }

    public void AddNode(Node node)
    {
        Functions[_activeFunction].Nodes.Add(node);
    }

    public Node GetNode(string nextMainNode) => Functions[_activeFunction].Nodes.Find(x => x.ID == nextMainNode);
}