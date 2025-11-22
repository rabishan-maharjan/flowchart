using System.Threading.Tasks;

public class BreakNodeObject : CommandObject
{
    public override Node Node => _node ??= new BreakCommand();
    protected override Task OpenCommandUi() => Task.CompletedTask;
}