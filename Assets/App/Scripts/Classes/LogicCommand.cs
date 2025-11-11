using System.Threading.Tasks;
using Arcube;
using UnityEngine;

public class LogicCommand : Command
{
    public string NodeTrue;
    public string NodeFalse;
    
    public string Variable1;
    public string Variable2;
    public string Operator;
    public LogicCommand()
    {
        Name = "LogicCommand";
    }

    public override async Task Execute()
    {
        Debug.Log("Executing " + Name);
        
        var flowChartManager = AppManager.GetManager<FlowChartManager>();
        var v1 = flowChartManager.VariableMap[Variable1];
        var v2 = flowChartManager.VariableMap[Variable2];
        //check expression
        var expression = OperatorHandler.OperateLogic(v1, v2, Operator);
        //do something
        if (expression)
        {
            var node = AppManager.GetManager<FlowChartManager>().GetNode(NodeTrue);
            if (node is Command command)
            {
                await command.Execute();
            }
        }
        else
        {
            var node = AppManager.GetManager<FlowChartManager>().GetNode(NodeFalse);
            if (node is Command command)
            {
                await command.Execute();
            }
        }
        
        Completed = true;
    }

    public override string GetDescription()
    {
        var flowChartManager = AppManager.GetManager<FlowChartManager>();
        var v1 = flowChartManager.VariableMap[Variable1];
        var v2 = flowChartManager.VariableMap[Variable2];

        return v1.Name + " " + Operator + v2.Name;
    }
}