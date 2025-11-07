public class LogicNodeObject : CommandObject
{
    public override Node Node => _node ??= new LogicCommand();
    protected override void OpenCommandUi()
    {
    }
}