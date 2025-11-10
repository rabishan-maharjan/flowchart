using System.Threading.Tasks;
using Arcube;
using Arcube.UiManagement;
using UnityEngine;

public class LoopCommandUi : CommandUi
{
    [SerializeField] private InputFieldSimple ip_count;
    [SerializeField] private ToggleButton tb_reverse;
    [SerializeField] private InputFieldSimple ip_step;
    protected override void Reset()
    {
        transform.TryFindObject(nameof(ip_count), out ip_count);
        transform.TryFindObject(nameof(tb_reverse), out tb_reverse);
        transform.TryFindObject(nameof(ip_step), out ip_step);
        
        base.Reset();
    }

    public override Task Open(Command command)
    {
        var loopCommand = (LoopCommand)command;
        ip_count.Text = loopCommand.Count.ToString();
        tb_reverse.IsOn = loopCommand.Reverse;
        ip_step.Text = loopCommand.Steps.ToString();
        
        return base.Open(command);
    }

    protected override void Apply()
    {
        var loopCommand = (LoopCommand)Command;
        
        int.TryParse(ip_count.Text, out loopCommand.Count);
        int.TryParse(ip_step.Text, out loopCommand.Steps);
        loopCommand.Reverse = tb_reverse.IsOn;
        
        base.Apply();
    }
}