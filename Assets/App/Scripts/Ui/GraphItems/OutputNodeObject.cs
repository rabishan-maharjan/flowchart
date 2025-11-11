using System.Threading.Tasks;
using Arcube;

public class OutputNodeObject : CommandObject
{
    public override Node Node => _node ??= new OutputCommand();

    protected override async Task OpenCommandUi()
    {
        await UiManager.GetUi<OutputCommandUi>().Open((Command)Node);
        await base.OpenCommandUi();
    }
}