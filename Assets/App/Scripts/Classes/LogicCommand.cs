using System.Collections.Generic;
using System.Threading.Tasks;
using Arcube;
using UnityEngine;

public class LogicExpression
{
    public string Variable1;
    public string Variable2;
    public string Operator;
    public string ConjunctionOperator;

    public bool Execute()
    {
        var flowChartManager = AppManager.GetManager<FlowChartManager>();
        var v1 = flowChartManager.VariableMap[Variable1];
        var v2 = flowChartManager.VariableMap[Variable2];

        return OperatorHandler.OperateLogic(v1, v2, Operator);
    }
}

public class LogicCommand : Command
{
    public string NodeTrue;
    public string NodeFalse;

    public readonly List<LogicExpression> Expressions = new();

    public LogicCommand()
    {
        Name = "LogicCommand";
    }

    public override async Task Execute()
    {
        Debug.Log("Executing " + Name);

        var overallResult = Expressions.Count <= 0 || Expressions[0].Execute();

        for (var i = 1; i < Expressions.Count; i++)
        {
            var currentResult = Expressions[i].Execute();
            var prevConjunction = Expressions[i - 1].ConjunctionOperator;

            if (prevConjunction == "and")
            {
                overallResult = overallResult && currentResult;
                if (!overallResult) break; // Short-circuit for AND
            }
            else if (prevConjunction == "or")
            {
                overallResult = overallResult || currentResult;
                if (overallResult) break; // Short-circuit for OR
            }
        }

        if (overallResult)
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
        var output = "";
        foreach (var expression in Expressions)
        {
            var v1 = flowChartManager.VariableMap[expression.Variable1];
            var v2 = flowChartManager.VariableMap[expression.Variable2];

            output += $"{v1.Name} {expression.Operator} {v2.Name} \n";
        }

        return string.IsNullOrEmpty(output) ? "Logic" : output;
    }
}