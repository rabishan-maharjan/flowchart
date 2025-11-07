using Arcube;

public class OutputNodeObject : CommandObject
{
    public override Node Node => _node ??= new OutputCommand();

    protected override void OpenCommandUi()
    {
        UiManager.GetUi<OutputCommandUi>().Open((Command)Node);
    }
}