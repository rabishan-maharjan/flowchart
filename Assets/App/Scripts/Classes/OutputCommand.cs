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
     
        //Debug.Log($"Output: {Output}");
        Function.OnOutput.Invoke(Output);
        
        await Wait(cts);
        Completed = true;
    }

    public override string GetDescription()
    {
        var output = "Output\n";
        foreach (var variable in Variables)
        {
            output += Variable.TryGetVariable(variable)?.Name;
        }
        
        return output;
    }
    
    public override string GetValue()
    {
        var output = "Output\n";
        foreach (var variable in Variables)
        {
            var v = Variable.TryGetVariable(variable);
            if(v == null) continue;
            output += v.Name + " = " + v.Value;
        }
        
        return output;
    }
}