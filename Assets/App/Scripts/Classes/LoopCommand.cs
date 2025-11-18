using System.Threading.Tasks;
using Arcube;
using UnityEngine;

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
                await ExecuteLoopItems();
            }
        }
        else
        {
            for (var i = 0; i < Count; i++)
            {
                await ExecuteLoopItems();
            }    
        }
        
        Completed = true;
    }
    
    private async Task ExecuteLoopItems()
    {
        var node = AppManager.GetManager<FlowChartManager>().GetNode(NodeLoop);
        while (node != null)
        {
            if (node is Command command)
            {
                //Debug.Log($"Executing {command.Name} from loop");
                await command.Execute();
            }

            if (!string.IsNullOrEmpty(node.NextNode))
            {
                node = AppManager.GetManager<FlowChartManager>().GetNode(node.NextNode);
            }
            else
            {
                break;
            }
        }
    }

    public override string GetDescription()
    {
        return $"Loop {Count / Steps}";
    }
}