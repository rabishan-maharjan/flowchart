using System;
using System.Threading.Tasks;
using Arcube;

public class InputCommand : Command
{
    public InputCommand()
    {
        Name = "InputCommand";    
    }
    
    public string Variable { get; set; }
    public override async Task Execute()
    {
        if (string.IsNullOrEmpty(Variable))
        {
            throw new Exception("Variable is null");
        }
        
        await base.Execute();
    }

    public override string GetDescription()
    {
        var v = AppManager.GetManager<FlowChartManager>().VariableMap[Variable];
        return $"Input {v.Name}";
    }
}