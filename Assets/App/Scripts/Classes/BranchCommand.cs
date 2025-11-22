using System.Collections.Generic;
using Arcube;

public abstract class BranchCommand : Command
{
    public List<string> LocalVariables { get; private set; } = new();

    public BranchCommand()
    {
        AppManager.GetManager<FlowChartManager>().Branches.Add(ID);
    }

    public void AddLocalVariable(Variable variable)
    {
        if (variable.Exposed) return;
        AddLocalVariable(variable.ID);
        var flowChartManager = AppManager.GetManager<FlowChartManager>();
        flowChartManager.Branches.Add(ID);
        flowChartManager.AddVariable(variable);
    }

    public void AddLocalVariable(string variable)
    {
        if (!LocalVariables.Contains(variable)) LocalVariables.Add(variable);
    }

    public void RemoveLocalVariable(string variable)
    {
        if (LocalVariables.Contains(variable)) LocalVariables.Remove(variable);
    }
}