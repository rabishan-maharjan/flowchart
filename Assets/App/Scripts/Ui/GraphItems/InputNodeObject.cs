using Arcube;

public class InputNodeObject : CommandObject
{
    public override Node Node => _node ??= new InputCommand();
    
    protected override void OpenCommandUi()
    {
        UiManager.GetUi<InputCommandUi>().Open((Command)Node);
    }
}