using System.Threading.Tasks;
using Arcube;

public class OperationNodeObject : CommandObject
{
    public override Node Node => _node ??= new OperationCommand();
    
    protected override async Task OpenCommandUi()
    {
        await UiManager.GetUi<OperationCommandUi>().Open((OperationCommand)Node);
        await base.OpenCommandUi();
    }
}