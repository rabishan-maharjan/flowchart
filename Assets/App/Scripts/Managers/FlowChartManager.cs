using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Arcube;
using Arcube.UiManagement;

public enum CompileState
{
    Work,
    Compile,
    Run
}

public enum ProjectState
{
    New,
    Save,
    Load,
}

public enum ExecutionType
{
    Normal,
    TwoSeconds,
    KeyPress,
}

public class FlowChartManager : ManagerBase
{
    private const string ActiveFunction = "Main";
    public Dictionary<string, Variable> VariableMap { get; set; } = new();
    public Dictionary<string, Function> Functions { get; private set; } = new();
    public List<Variable> ActiveVariables => Functions[ActiveFunction].Variables;
    public List<Node> ActiveNodes => Functions[ActiveFunction].Nodes;
    public List<string> Branches { get; set; } = new();
    public ExecutionType ExecutionType { get; set; } = ExecutionType.Normal;
    public event Action<ProjectState, string> OnProjectStateChanged;
    public event Action<CompileState> OnCompileStateChanged;
    public override Task<bool> Initialize()
    {
        New();
        return base.Initialize();
    }

    public void ClearNodes()
    {
        foreach (var function in Functions)
        {
            function.Value.Nodes.Clear();
        }
    }
    
    public string CurrentFile { get; private set; }
    public void New()
    {
        Functions.Clear();
        Functions.Add(ActiveFunction, new Function());
        Branches.Clear();
        VariableMap.Clear();
        CurrentFile = "";
        OnProjectStateChanged?.Invoke(ProjectState.New, CurrentFile);
    }

    public void Save(string fileName)
    {
        CurrentFile = fileName;
        AppManager.GetManager<IOManager>().Save(fileName + Ext, Functions);
        OnProjectStateChanged?.Invoke(ProjectState.Save, CurrentFile);
    }
    
    public void Work()
    {
        OnCompileStateChanged?.Invoke(CompileState.Work);
    }

    public const string Ext = ".flw";
    public void Load(string fileName)
    {
        CurrentFile = fileName;
        Functions = AppManager.GetManager<IOManager>().Load($"{fileName}{Ext}");
        foreach (var function in Functions)
        {
            foreach (var variable in function.Value.Variables)
            {
                AddVariable(variable);
            }
        }
        
        Branches.Clear();
        foreach (var variable in ActiveVariables)
        {
            if(!Branches.Contains(variable.BranchID)) Branches.Add(variable.BranchID);
        }
        
        OnProjectStateChanged?.Invoke(ProjectState.Load, CurrentFile);
    }
    
    public void Compile()
    {
        OnCompileStateChanged?.Invoke(CompileState.Compile);
    }
    
    private CancellationTokenSource _cts;
    public async void Run()
    {
        try
        {
            _cts?.Dispose();
            _cts = new CancellationTokenSource();
            OnCompileStateChanged?.Invoke(CompileState.Run);
            await Functions["Main"].Execute(_cts);
        }
        catch (Exception e)
        {
            Log.AddException(e);
        }
    }

    public void AddVariable(Variable variable)
    {
        var variables = Functions[ActiveFunction].Variables;
        VariableMap.TryAdd(variable.ID, variable);
        if(!variables.Contains(variable))
        {
            variables.Add(variable);
        }
    }
    
    public void RemoveVariable(Variable variable)
    {
        foreach (var function in Functions)
        {
            foreach (var node in function.Value.Nodes)
            {
                if (!node.IsVariableUsed(variable.ID)) continue;
                MessageUi.Show("Variable is being used. Cannot delete!");
                return;
            }
        }
        
        var variables = Functions[ActiveFunction].Variables;
        variables.Remove(variable);
    }
    
    public void AddNode(Node node)
    {
        Functions[ActiveFunction].Nodes.Add(node);
    }

    public Node GetNode(string nextMainNode) => Functions[ActiveFunction].Nodes.Find(x => x.ID == nextMainNode);

    public void StopExecution()
    {
        OnCompileStateChanged?.Invoke(CompileState.Work);
        _cts.Cancel();
    }
}