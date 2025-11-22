using System.Threading;
using System.Threading.Tasks;

public class BreakCommand : Command
{
    public BreakCommand()
    {
        Name = "BreakCommand";
    }

    public override async Task<bool> Execute(CancellationTokenSource cts)
    {
        await base.Execute(cts);

        await Task.Run(() => { });
        Completed = true;

        OnExecuteEnd?.Invoke();
        return false;
    }

    public override string GetDescription() => "Break";

    public override string GetValueDescription() => "Break";
}