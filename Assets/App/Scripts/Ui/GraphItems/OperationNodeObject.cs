public class OperationNodeObject : CommandObject
{
    public override Node Node => _node ??= new OperationCommand();
    
    protected override void OpenCommandUi()
    {
    }
}