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

    public string Start;
    public string End;
    public string Steps;
    public override async Task Execute(CancellationTokenSource cts)
    {
        OnExecuteStart?.Invoke();
        //get all commands after this command
        //execute them
        var flowChartManager = AppManager.GetManager<FlowChartManager>(); 
        Variable v = null;
        if(!string.IsNullOrEmpty(Variable)) v = flowChartManager.VariableMap[Variable];
        
        var start = Convert.ToInt32(flowChartManager.VariableMap[Start].GetValue());
        var end = Convert.ToInt32(flowChartManager.VariableMap[End].GetValue());
        var steps = Convert.ToInt32(flowChartManager.VariableMap[Steps].GetValue());
        
        var reverse = start > end;
        if (reverse)
        {
            for (var i = start; i >= end; i--)
            {
                if (v != null)
                {
                    v.Value = (i * steps).ToString();
                }
                await ExecuteLoopItems(cts);
            }
        }
        else
        {
            for (var i = start; i <= end; i++)
            {
                if (v != null)
                {
                    v.Value = (i * steps).ToString();
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
        var start = global::Variable.TryGetVariable(Start).Value;
        output += $" Start {start}\n";
        var end = global::Variable.TryGetVariable(End).Value;
        output += $" End {end}\n";
        var steps = global::Variable.TryGetVariable(Steps).Value;
        output += $" Steps {steps}\n";
        return output;
    }
    
    public override string GetValueDescription()
    {
        var output = "Loop";
        var v = global::Variable.TryGetVariable(Variable);
        if(v != null) output += $" {v.Name}:{v.Value}\n";
        else output += "\n";
        var start = global::Variable.TryGetVariable(Start).Value;
        output += $" Start {start}\n";
        var end = global::Variable.TryGetVariable(End).Value;
        output += $" End {end}\n";
        var steps = global::Variable.TryGetVariable(Steps).Value;
        output += $" Steps {steps}\n";
        return output;
    }
}