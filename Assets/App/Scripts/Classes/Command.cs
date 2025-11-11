using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Arcube;
using Newtonsoft.Json;
using UnityEngine;

public abstract class Command : Node
{
    public bool Completed { get; set; } = false;
    public virtual async Task Execute()
    {
        while (!Completed) await Task.Yield();
    }
}

public class OutputCommand : Command
{
    public List<string> Variables { get; set; } = new();
    public OutputCommand()
    {
        Name = "OutputCommand";
    }

    [JsonIgnore] private string Output { get; set; }
    public override Task Execute()
    {
        Output = "";
        foreach (var variable in Variables)
        {
            var v = AppManager.GetManager<FlowChartManager>().VariableMap[variable];
            Output += v.Value;
        }
        
        Function.OnOutput.Invoke(Output);
        
        Completed = true;
        return Task.CompletedTask;
    }
}

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
}

public class OperationCommand : Command
{
    public string Assignee { get; set; }
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
        flowChartManager.VariableMap[Assignee].Value = result.Value;
        Completed = true;
        return Task.CompletedTask;
    }
}

public class LogicCommand : Command
{
    public string NodeTrue;
    public string NodeFalse;
    
    public string Expression1;
    public string Expression2;
    public string Operator;
    public LogicCommand()
    {
        Name = "LogicCommand";
    }

    public override async Task Execute()
    {
        Debug.Log("Executing " + Name);
        
        var flowChartManager = AppManager.GetManager<FlowChartManager>();
        var e1 = flowChartManager.VariableMap[NodeTrue];
        var e2 = flowChartManager.VariableMap[NodeFalse];
        //check expression
        var expression = OperatorHandler.OperateLogic(e1, e2, Operator);
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
}

public class LoopCommand : Command
{
    public string NodeLoop;
    public LoopCommand()
    {
        Name = "LoopCommand";
    }
    
    public bool Reverse = false;
    public int Count = 0;
    public int Steps = 1;
    public override async Task Execute()
    {
        //get all commands after this command
        //execute them
        if (Reverse)
        {
            for (var i = Count - 1; i >= 0; i -= Steps)
            {
                await ExecuteLoopItem();
            }
        }
        else
        {
            for (var i = 0; i < Count; i++)
            {
                await ExecuteLoopItem();
            }    
        }
        
        var node = AppManager.GetManager<FlowChartManager>().GetNode(NextNode);
        if (node is Command command)
        {
            await command.Execute();
        }
        
        Completed = true;
    }
    
    private async Task ExecuteLoopItem()
    {
        var node = AppManager.GetManager<FlowChartManager>().GetNode(NodeLoop);
        if (node is Command command)
        {
            await command.Execute();
        }
    }
}