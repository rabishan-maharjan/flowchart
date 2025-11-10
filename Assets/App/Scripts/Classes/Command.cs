using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Arcube;
using Newtonsoft.Json;
using UnityEngine;

public enum ExpressionType
{
    Select,
    Text,
    Number,
    Variable,
    Function,
    Operator,
}

public enum Operator
{
    Add,
    Sub,
    Mul,
    Div,
    Mod,
    Pow,
    And,
    Or,
    Not,
    Equal,
    NotEqual,
    Less,
    LessEqual,
    Greater,
    GreaterEqual,
}

public class ExpressionPair
{
    public ExpressionType Type;
    public object Value;
}

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
    public List<Variable> Variables { get; set; } = new();
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
            Output += variable.Value;
        }
        
        Completed = true;
        Function.OnOutput.Invoke(Output);
        
        return Task.CompletedTask;
    }
}

public class InputCommand : Command
{
    public InputCommand()
    {
        Name = "InputCommand";    
    }
    
    public Variable Variable { get; set; }
    public override async Task Execute()
    {
        if (Variable == null)
        {
            throw new Exception("Variable is null");
        }
        
        await base.Execute();
    }
}

public class OperationCommand : Command
{
    public List<ExpressionPair> Expressions { get; set; } = new();
    public OperationCommand()
    {
        Name = "OperationCommand";
        Expressions = new();
    }
    
    public override Task Execute()
    {
        foreach (var expression in Expressions)
        {
            //do something
        }
        
        Completed = true;
        return Task.CompletedTask;
    }
}

public class LogicCommand : Command
{
    public string NodeTrue;
    public string NodeFalse;
    
    public Variable Expression1;
    public Variable Expression2;
    public Operator Operator;
    public LogicCommand()
    {
        Name = "LogicCommand";
    }

    public override async Task Execute()
    {
        Debug.Log("Executing " + Name);
        
        //check expression
        var expression = Utils.RandomBool;
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