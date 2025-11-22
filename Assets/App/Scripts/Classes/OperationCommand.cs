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
    public OperationCommand()
    {
        Name = "OperationCommand";
    }
    
    public List<Expression> Expressions { get; set; } = new();
    public override bool IsVariableUsed(string variable) => Variable == variable || Expressions.Exists(x => x.Variable == variable);

    public override async Task<bool> Execute(CancellationTokenSource cts)
    {
        try
        {
            OnExecuteStart?.Invoke();
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
            OnExecuteEnd?.Invoke();
        }
        catch (Exception e)
        {
            Log.AddException(e);
        }
        
        return true;
    }

    public override string GetDescription()
    {
        var flowChartManager = AppManager.GetManager<FlowChartManager>();
        var v = global::Variable.TryGetVariable(Variable);
        if(v == null) return "Operation";
        
        var output = $"{v.Name} = ";
        foreach (var expression in Expressions)
        {
            var v1 = flowChartManager.VariableMap[expression.Variable];
            output += $"{v1.Name}";
            if(!string.IsNullOrEmpty(expression.Operator)) output += $" {expression.Operator} ";
        }

        return string.IsNullOrEmpty(output) ? "Operation" : output;
    }
    
    public override string GetValueDescription()
    {
        var flowChartManager = AppManager.GetManager<FlowChartManager>();
        var v = flowChartManager.VariableMap[Variable];
        Description = GetDescription() + "\n";
        Description += $"{v.Name}:{v.Value}";

        return string.IsNullOrEmpty(Description) ? "Operation" : Description;
    }
}