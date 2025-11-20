using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Arcube;
using Newtonsoft.Json;

public class OutputCommand : Command
{
    public List<string> Variables { get; set; } = new();
    public OutputCommand()
    {
        Name = "OutputCommand";
    }
    [JsonIgnore] private string Output { get; set; }
    public override async Task Execute(CancellationTokenSource cts)
    {
        OnExecuteStart?.Invoke();
        Output = "";
        foreach (var variable in Variables)
        {
            var v = AppManager.GetManager<FlowChartManager>().VariableMap[variable];
            if (v.Type == VariableType.Number)
            {
                // round to two decimals and remove unnecessary zeros
                var num = float.Parse(v.Value);
                Output += num.ToString("0.##");   // "0.##" removes trailing zeros
            }
            else
            {
                Output += v.Value;
            }
        }
        
        Function.OnOutput.Invoke(Output);
        
        await Wait(cts);
        Completed = true;
        OnExecuteEnd?.Invoke();
    }

    public override string GetDescription()
    {
        Description = "Output\n";
        foreach (var variable in Variables)
        {
            Description += Variable.TryGetVariable(variable)?.Name + " + ";
        }
        
        if (Variables.Count > 0) Description = Description[..^3];
        
        return Description;
    }
    
    public override string GetValueDescription()
    {
        Description = GetDescription() + "\n";
        foreach (var variable in Variables)
        {
            var v = Variable.TryGetVariable(variable);
            if(v.Exposed) Description += v.Name + ":" + v.Value + ",";
        }
        
        Description = Description.TrimEnd(',');
        
        return Description;
    }
}