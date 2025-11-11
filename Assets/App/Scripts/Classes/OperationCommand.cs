using System.Threading.Tasks;
using Arcube;

public class OperationCommand : Command
{
    public string Variable { get; set; }
    public string Variable1 { get; set; }
    public string Variable2 { get; set; }
    public string Operator { get; set; }
    public OperationCommand()
    {
        Name = "OperationCommand";
    }
    
    public override Task Execute()
    {
        var flowChartManager = AppManager.GetManager<FlowChartManager>();
        var v1 = flowChartManager.VariableMap[Variable1];
        var v2 = flowChartManager.VariableMap[Variable2];
        var result = OperatorHandler.OperateArithmetic(v1, v2, Operator);
        flowChartManager.VariableMap[Variable].Value = result.Value;
        Completed = true;
        return Task.CompletedTask;
    }

    public override string GetDescription()
    {
        var flowChartManager = AppManager.GetManager<FlowChartManager>();
        var v = flowChartManager.VariableMap[Variable];
        return $"Operating on {v.Name}";
    }
}