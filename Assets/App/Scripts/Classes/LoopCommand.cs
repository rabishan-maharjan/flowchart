using System.Threading;
using System.Threading.Tasks;
using Arcube;

public class LoopCommand : Command
{
    public string NodeLoop;
    public LoopCommand()
    {
        Name = "LoopCommand";
    }

    public string Variable;
    public bool Reverse = false;
    public int Count = 0;
    public int Steps = 1;
    public override async Task Execute(CancellationTokenSource cts)
    {
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
            for (var i = 0; i < Count; i += Steps)
            {
                if (v != null)
                {
                    v.Value = i.ToString();
                }
                await ExecuteLoopItems(cts);
            }    
        }
     
        Completed = true;
    }
    
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
        output += $" Reverse {Reverse}\n";
        return output;
    }
    
    public override string GetValue()
    {
        var output = "Loop";
        var v = global::Variable.TryGetVariable(Variable);
        if(v != null) output += $" {v.Name}:{v.Value}\n";
        else output += "\n";
        output += $" Count {Count}\n";
        output += $" Steps {Steps}\n";
        output += $" Reverse {Reverse}\n";
        return output;
    }
}