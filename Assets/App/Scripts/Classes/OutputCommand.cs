using System.Collections.Generic;
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
    public override Task Execute()
    {
        Output = "";
        foreach (var variable in Variables)
        {
            var v = AppManager.GetManager<FlowChartManager>().VariableMap[variable];
            Output += v.Value;
        }
        
        Function.OnOutput.Invoke(Output);
        
        Completed = true;
        return Task.CompletedTask;
    }

    public override string GetDescription()
    {
        return "Output";
    }
}