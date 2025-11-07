using Arcube;
using Arcube.UiManagement;

public class LoopCommandUi : CommandUi
{
    protected override void Apply()
    {
        var loops = int.Parse(gameObject.FindObject<InputFieldSimple>("ip_loops").Text);
        var reverse = gameObject.FindObject<ToggleButton>("tb_reverse").IsOn;
        var steps = int.Parse(gameObject.FindObject<InputFieldSimple>("ip_steps").Text);

        var loopCommand = (LoopCommand)Command;
        loopCommand.Count = loops;
        loopCommand.Reverse = reverse;
        loopCommand.Steps = steps;
        
        base.Apply();
    }
}