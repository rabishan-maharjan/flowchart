using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Arcube;
using Newtonsoft.Json;

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

    [JsonIgnore] private bool _result;
    public override async Task Execute(CancellationTokenSource cts)
    {
        OnExecuteStart?.Invoke();

        _result = Expressions.Count <= 0 || Expressions[0].Execute();

        for (var i = 1; i < Expressions.Count; i++)
        {
            var currentResult = Expressions[i].Execute();
            var prevConjunction = Expressions[i - 1].ConjunctionOperator;

            if (prevConjunction == "and")
            {
                _result = _result && currentResult;
                if (!_result) break; // Short-circuit for AND
            }
            else if (prevConjunction == "or")
            {
                _result = _result || currentResult;
                if (_result) break; // Short-circuit for OR
            }
        }

        await Wait(cts);
        if (_result)
        {
            var node = AppManager.GetManager<FlowChartManager>().GetNode(NodeTrue);
            if (node is Command command)
            {
                await ExecuteBranchItems(command, cts);
            }
        }
        else
        {
            var node = AppManager.GetManager<FlowChartManager>().GetNode(NodeFalse);
            if (node is Command command)
            {
                await ExecuteBranchItems(command, cts);
            }
        }

        await Wait(cts);
        Completed = true;
        OnExecuteEnd?.Invoke();
    }
    
    private async Task ExecuteBranchItems(Node node, CancellationTokenSource cts)
    {
        var flowChartManager = AppManager.GetManager<FlowChartManager>();
        while (node != null)
        {
            if (node is Command command)
            {
                await command.Execute(cts);
                await Task.Yield();
            }

            if (!string.IsNullOrEmpty(node.NextNode))
            {
                node = flowChartManager.GetNode(node.NextNode);
            }
            else
            {
                break;
            }
        }
    }

    public override string GetDescription()
    {
        var flowChartManager = AppManager.GetManager<FlowChartManager>();
        var output = "";
        foreach (var expression in Expressions)
        {
            var v1 = flowChartManager.VariableMap[expression.Variable1];
            var v2 = flowChartManager.VariableMap[expression.Variable2];

            output += $"{v1.Name} {expression.Operator} {v2.Name}";
            if(!string.IsNullOrEmpty(expression.ConjunctionOperator)) output += $" {expression.ConjunctionOperator}\n";
        }

        return string.IsNullOrEmpty(output) ? "Logic" : output;
    }
    
    public override string GetValueDescription()
    {
        var flowChartManager = AppManager.GetManager<FlowChartManager>();
        var output = GetDescription() + "\n";
        foreach (var expression in Expressions)
        {
            var v1 = flowChartManager.VariableMap[expression.Variable1];
            var v2 = flowChartManager.VariableMap[expression.Variable2];

            output += $"({v1.Name}:{v1.Value} {expression.Operator}";
            if(v2.Exposed) output += $" {v2.Name}:{v2.Value})";
            else output += $" {v2.Value})";
            
            if(!string.IsNullOrEmpty(expression.ConjunctionOperator)) output += $" {expression.ConjunctionOperator}\n";
        }
        
        output += $" = {_result}";

        return string.IsNullOrEmpty(output) ? "Logic" : output;
    }
}