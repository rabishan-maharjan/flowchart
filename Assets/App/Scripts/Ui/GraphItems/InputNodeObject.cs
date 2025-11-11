using System.Threading.Tasks;
using Arcube;

public class InputNodeObject : CommandObject
{
    public override Node Node => _node ??= new InputCommand();
    
    protected override async Task OpenCommandUi()
    {
        await UiManager.GetUi<InputCommandUi>().Open((Command)Node);
        await base.OpenCommandUi();
    }
}