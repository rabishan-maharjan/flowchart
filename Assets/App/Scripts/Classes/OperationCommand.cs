using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
    
    public override async Task Execute(CancellationTokenSource cts)
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

            await Wait(cts);
            Completed = true;
        }
        catch (Exception e)
        {
            Log.AddException(e);
        }
    }

    public override string GetDescription()
    {
        var flowChartManager = AppManager.GetManager<FlowChartManager>();
        var v = flowChartManager.VariableMap[Variable];
        var output = $"{v.Name} = ";
        foreach (var expression in Expressions)
        {
            var v1 = flowChartManager.VariableMap[expression.Variable];
            output += $"{v1.Name}";
            if(!string.IsNullOrEmpty(expression.Operator)) output += $" {expression.Operator} ";
        }

        return string.IsNullOrEmpty(output) ? "Operation" : output;
    }
    
    public override string GetValue()
    {
        var flowChartManager = AppManager.GetManager<FlowChartManager>();
        var v = flowChartManager.VariableMap[Variable];
        var output = $"{v.Name}:{v.Value} = ";
        foreach (var expression in Expressions)
        {
            var v1 = flowChartManager.VariableMap[expression.Variable];
            output += $"{v1.Name}:{v1.Value}";
            if(!string.IsNullOrEmpty(expression.Operator)) output += $" {expression.Operator} ";
        }

        return string.IsNullOrEmpty(output) ? "Operation" : output;
    }
}