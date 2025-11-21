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
    public int Start = 0;
    public int Steps = 1;
    public override async Task Execute(CancellationTokenSource cts)
    {
        OnExecuteStart?.Invoke();
        //get all commands after this command
        //execute them
        Variable v = null;
        if(!string.IsNullOrEmpty(Variable)) v = AppManager.GetManager<FlowChartManager>().VariableMap[Variable];
        var delta = Start / Steps;
        if (Reverse)
        {
            for (var i = Steps; i > 0; i--)
            {
                if (v != null)
                {
                    v.Value = (i * delta).ToString();
                }
                await ExecuteLoopItems(cts);
            }
        }
        else
        {
            for (var i = 1; i <= Steps; i++)
            {
                if (v != null)
                {
                    v.Value = (i * delta).ToString();
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
        output += $" Start {Start}\n";
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
        output += $" From {Start} to 0\n";
        output += $" Steps {Steps}\n";
        return output;
    }
}