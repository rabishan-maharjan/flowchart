using System.Threading.Tasks;
using Arcube;
using Arcube.UiManagement;

public abstract class CommandUi : Ui
{
    protected override void Reset()
    {
        deactivateOnClose = true;
        base.Reset();
    }

    protected virtual void Start()
    {
        gameObject.FindObject<ButtonImage>("b_close").OnClick.AddListener(Close);
        gameObject.FindObject<ButtonImage>("b_ok").OnClick.AddListener(Apply);
    }

    protected Command Command { get; private set; }

    public virtual Task Open(Command command)
    {
        Command = command;
        return OpenAsync();
    }

    protected virtual void Apply()
    {
        Close();
    }
}