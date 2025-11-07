using System;
using System.Collections.Generic;

public class ExpressionPair
{
    public ExpressionType Type;
    public object Value;
}

public abstract class Command : Node
{
    public abstract void Execute();
}

public class OutputCommand : Command
{
    public Variable Variable { get; set; }
    public OutputCommand()
    {
        Name = "OutputCommand";
    }

    public string Output { get; set; }
    public override void Execute()
    {
        if (Variable == null)
        {
            throw new Exception("Variable is null");
        }
        Output = Variable.Value;
    }
}

public class InputCommand : Command
{
    public InputCommand()
    {
        Name = "InputCommand";    
    }
    
    public Variable Variable;
    public override void Execute()
    {
        if (Variable == null)
        {
            throw new Exception("Variable is null");
        }
    }
}

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

public class OperationCommand : Command
{
    public List<ExpressionPair> Expressions { get; set; } = new();
    public OperationCommand()
    {
        Name = "OperationCommand";
        Expressions = new();
    }
    
    public override void Execute()
    {
        foreach (var expression in Expressions)
        {
            //do something
        }
    }
}

public class LogicCommand : Command
{
    public LogicCommand()
    {
        Name = "LogicCommand";
    }

    public override void Execute()
    {
        throw new NotImplementedException();
    }
}

public class LoopCommand : Command
{
    public LoopCommand()
    {
        Name = "LoopCommand";
    }
    
    public bool Reverse;
    public int Count;
    public int Steps;
    public override void Execute()
    {
        throw new NotImplementedException();
    }
}