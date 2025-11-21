using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Arcube;
using Newtonsoft.Json;

public class WhileLoopCommand : Command
{
    public string NodeLoop;
    public WhileLoopCommand()
    {
        Name = "WhileLoopCommand";
    }

    [JsonIgnore] private bool _result;
    public readonly List<LogicExpression> Expressions = new();
    
    public override bool IsVariableUsed(string variable) => Expressions.Exists(x => x.Variable1 == variable || x.Variable2 == variable);

    public override async Task Execute(CancellationTokenSource cts)
    {
        OnExecuteStart?.Invoke();
        //get all commands after this command
        //execute them
        _result = CalculateExpressions();
        while(_result)
        {
            await ExecuteLoopItems(cts);
            _result = CalculateExpressions();
        };

        Completed = true;
        OnExecuteEnd?.Invoke();
    }

    private bool CalculateExpressions()
    {
        var result = Expressions.Count <= 0 || Expressions[0].Execute();
        for (var i = 1; i < Expressions.Count; i++)
        {
            var currentResult = Expressions[i].Execute();
            var prevConjunction = Expressions[i - 1].ConjunctionOperator;

            if (prevConjunction == "and")
            {
                result = result && currentResult;
                if (!result) break; // Short-circuit for AND
            }
            else if (prevConjunction == "or")
            {
                result = result || currentResult;
                if (result) break; // Short-circuit for OR
            }
        }

        return result;
    }

    [JsonIgnore] public Action OnLoopStep;
    private async Task ExecuteLoopItems(CancellationTokenSource cts)
    {
        var flowChartManager = AppManager.GetManager<FlowChartManager>();
        var node = flowChartManager.GetNode(NodeLoop);
        while (node != null)
        {
            if (node is Command command)
            {
                //Debug.Log($"Executing {command.Name} from loop");
                await command.Execute(cts);
                OnLoopStep?.Invoke();
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

        return string.IsNullOrEmpty(output) ? "While" : output;
    }
    
    public override string GetValueDescription()
    {
        var output = GetDescription() + "\n";
        output += $"{_result}";

        return string.IsNullOrEmpty(output) ? "Loop" : output;
    }
}