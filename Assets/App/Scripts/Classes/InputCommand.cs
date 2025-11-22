using System;
using System.Threading;
using System.Threading.Tasks;

public class InputCommand : Command
{
    public InputCommand()
    {
        Name = "InputCommand";    
    }
    
    public string Variable { get; set; }
    public override bool IsVariableUsed(string variable) => Variable == variable;

    public override async Task<bool> Execute(CancellationTokenSource cts)
    {
        await base.Execute(cts);
        
        Function.OnInput?.Invoke(Variable);
        if (string.IsNullOrEmpty(Variable))
        {
            throw new Exception("Variable is null");
        }
        
        while (!Completed)
        {
            await Task.Yield();
            cts.Token.ThrowIfCancellationRequested();
        }
        
        OnExecuteEnd?.Invoke();
        return true;
    }

    public override string GetDescription()
    {
        var v = global::Variable.TryGetVariable(Variable);
        return v == null ? "Input" : $"Input {v.Name}";
    }
    
    public override string GetValueDescription()
    {
        var v = global::Variable.TryGetVariable(Variable);
        if (v == null) return "Input";
        Description = $"{GetDescription()}\n{v.Value}";
        return Description;
    }
}