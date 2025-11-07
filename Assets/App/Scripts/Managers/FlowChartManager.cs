using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Arcube;

public class Variable
{
    public string Name;
    public string Type;
    public string Value;
    public bool Assigned;
}

public class Function
{
    public List<Variable> Variables { get; set; } = new(); 
    public List<Node> Nodes { get; set; } = new();

    public static event Action<Variable> OnInput;
    public static event Action<string> OnOutput;
    public static event Action<string> OnError;
    public void Execute()
    {
        try
        {
            foreach (var node in Nodes)
            {
                if (node is Command command)
                {
                    command.Execute();
                    if (command is InputCommand inputCommand) OnInput?.Invoke(inputCommand.Variable);
                    if (command is OutputCommand outputCommand) OnOutput?.Invoke(outputCommand.Output);
                }
            }
        }
        catch (Exception e)
        {
            OnError?.Invoke(e.Message);
            Log.AddError(()=> e.Message);
        }
    }
}

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

    public void AddNode(Node node, Node prevNode, bool isMain)
    {
        if (prevNode != null)
        {
            if(isMain) prevNode.NextMainNode = node.ID;
            else prevNode.NextSecondaryNode = node.ID;
        }
        
        Functions[_activeFunction].Nodes.Add(node);
    }
}