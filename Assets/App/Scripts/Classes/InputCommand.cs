using System;
using System.Threading;
using System.Threading.Tasks;
using Arcube;

public class InputCommand : Command
{
    public InputCommand()
    {
        Name = "InputCommand";    
    }
    
    public string Variable { get; set; }
    public override async Task Execute(CancellationTokenSource cts)
    {
        if (string.IsNullOrEmpty(Variable))
        {
            throw new Exception("Variable is null");
        }
        
        while (!Completed)
        {
            await Task.Yield();
            cts.Token.ThrowIfCancellationRequested();
        }
    }

    public override string GetDescription()
    {
        var v = AppManager.GetManager<FlowChartManager>().VariableMap[Variable];
        return $"Input {v.Name}";
    }
    
    public override string GetValue()
    {
        var v = AppManager.GetManager<FlowChartManager>().VariableMap[Variable];
        return $"Input {v.Name} = {v.Value}";
    }
}