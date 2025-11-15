using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Arcube;

public class Expression
{
    public string Variable;
    public string Operator;
    
    public static List<Expression> SortByDMAS(List<Expression> list)
    {
        int Priority(string op) => op switch
        {
            "*" or "/" or "%" => 0,
            "+" or "-" => 1,
            _ => 2
        };

        return list
            .OrderBy(e => Priority(e.Operator))
            .ToList();
    }
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
            var sortedExpressions = Expression.SortByDMAS(Expressions);
            for (var i = 0; i < sortedExpressions.Count; i++)
            {
                if (i == 0)
                {
                    result = new(flowChartManager.VariableMap[sortedExpressions[i].Variable]);
                }
                else
                {
                    var v2 = flowChartManager.VariableMap[sortedExpressions[i].Variable];
                    result = OperatorHandler.OperateArithmetic(result, v2, sortedExpressions[i - 1].Operator);
                }
                
                if(string.IsNullOrEmpty(sortedExpressions[i].Operator)) break;
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