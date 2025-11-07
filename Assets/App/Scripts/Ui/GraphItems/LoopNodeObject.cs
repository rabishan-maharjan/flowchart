public class LoopNodeObject : CommandObject
{
    public override Node Node => _node ??= new LoopCommand();
    protected override void OpenCommandUi()
    {
    }
}