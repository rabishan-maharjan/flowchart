using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Arcube;

public class Expression
{
    public string Variable;
    public string Operator;
}

public class OperationCommand : Command
{
    public string Variable { get; set; }
    public List<Expression> Expressions { get; set; } = new();
    public OperationCommand()
    {
        Name = "OperationCommand";
    }
    
    public override Task Execute()
    {
        try
        {
            var flowChartManager = AppManager.GetManager<FlowChartManager>();
            Variable result = new();
            for (var i = 0; i < Expressions.Count; i++)
            {
                if (i == 0)
                {
                    result = new(flowChartManager.VariableMap[Expressions[i].Variable]);
                }
                else
                {
                    var v2 = flowChartManager.VariableMap[Expressions[i].Variable];
                    result = OperatorHandler.OperateArithmetic(result, v2, Expressions[i - 1].Operator);
                }
                
                if(string.IsNullOrEmpty(Expressions[i].Operator)) break;
            }

            flowChartManager.VariableMap[Variable].Value = result.Value;

            Completed = true;
        }
        catch (Exception e)
        {
            Log.AddException(e);
        }
        
        return Task.CompletedTask;
    }

    public override string GetDescription()
    {
        var flowChartManager = AppManager.GetManager<FlowChartManager>();
        var v = flowChartManager.VariableMap[Variable];
        return $"Operating on {v.Name}";
    }
}