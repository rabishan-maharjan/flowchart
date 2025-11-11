using System.Threading.Tasks;
using Arcube;

public class LoopCommand : Command
{
    public string NodeLoop;
    public LoopCommand()
    {
        Name = "LoopCommand";
    }
    
    public bool Reverse = false;
    public int Count = 0;
    public int Steps = 1;
    public override async Task Execute()
    {
        //get all commands after this command
        //execute them
        if (Reverse)
        {
            for (var i = Count - 1; i >= 0; i -= Steps)
            {
                await ExecuteLoopItem();
            }
        }
        else
        {
            for (var i = 0; i < Count; i++)
            {
                await ExecuteLoopItem();
            }    
        }
        
        var node = AppManager.GetManager<FlowChartManager>().GetNode(NextNode);
        if (node is Command command)
        {
            await command.Execute();
        }
        
        Completed = true;
    }
    
    private async Task ExecuteLoopItem()
    {
        var node = AppManager.GetManager<FlowChartManager>().GetNode(NodeLoop);
        if (node is Command command)
        {
            await command.Execute();
        }
    }

    public override string GetDescription()
    {
        return $"Loop {Count / Steps}";
    }
}