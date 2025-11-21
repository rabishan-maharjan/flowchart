using System;
using System.Threading;
using System.Threading.Tasks;
using Arcube;
using Newtonsoft.Json;

public class ForLoopCommand : Command
{
    public string NodeLoop;
    public ForLoopCommand()
    {
        Name = "ForLoopCommand";
    }

    public string Variable;
    public override bool IsVariableUsed(string variable) => Variable == variable;

    public bool Reverse = false;
    public int Count = 0;
    public int Steps = 1;
    public override async Task Execute(CancellationTokenSource cts)
    {
        OnExecuteStart?.Invoke();
        //get all commands after this command
        //execute them
        Variable v = null;
        if(!string.IsNullOrEmpty(Variable)) v = AppManager.GetManager<FlowChartManager>().VariableMap[Variable]; 
        if (Reverse)
        {
            for (var i = Count; i > 0; i -= Steps)
            {
                if (v != null)
                {
                    v.Value = i.ToString();
                }
                await ExecuteLoopItems(cts);
            }
        }
        else
        {
            for (var i = 1; i <= Count; i += Steps)
            {
                if (v != null)
                {
                    v.Value = i.ToString();
                }
                await ExecuteLoopItems(cts);
            }    
        }
     
        Completed = true;
        OnExecuteEnd?.Invoke();
    }

    [JsonIgnore] public Action OnLoopStep;
    private async Task ExecuteLoopItems(CancellationTokenSource cts)
    {
        var flowChartManager = AppManager.GetManager<FlowChartManager>();
        var node = flowChartManager.GetNode(NodeLoop);
        while (node != null)
        {
            if (node is Command command)
            {
                //Debug.Log($"Executing {command.Name} from loop");
                await command.Execute(cts);
                await Task.Yield();
                OnLoopStep?.Invoke();
            }

            if (!string.IsNullOrEmpty(node.NextNode))
            {
                node = flowChartManager.GetNode(node.NextNode);
            }
            else
            {
                break;
            }
        }
    }

    public override string GetDescription()
    {
        var output = "Loop";
        var v = global::Variable.TryGetVariable(Variable);
        if(v != null) output += $" {v.Name}\n";
        else output += "\n";
        output += $" Count {Count}\n";
        output += $" Steps {Steps}\n";
        output += Reverse ? "Desc\n" : "Asc\n";
        return output;
    }
    
    public override string GetValueDescription()
    {
        var output = "Loop";
        var v = global::Variable.TryGetVariable(Variable);
        if(v != null) output += $" {v.Name}:{v.Value}\n";
        else output += "\n";
        output += $" Count {Count}\n";
        output += $" Steps {Steps}\n";
        output += Reverse ? "Desc\n" : "Asc\n";
        return output;
    }
}