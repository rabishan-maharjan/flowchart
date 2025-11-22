public abstract class BranchObject : CommandObject
{
    public override void Delete(bool force)
    {
        var branchNode = (BranchCommand)_node;
        foreach (var variable in branchNode.LocalVariables)
        {
            var v = FlowChartManager.VariableMap[variable];
            FlowChartManager.RemoveVariable(v);
        }
        
        FlowChartManager?.Branches.Remove(Node.ID);
        base.Delete(force);
    }
}