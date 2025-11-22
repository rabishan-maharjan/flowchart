public class StartNodeObject : NodeObject
{
    public override Node Node => _node ??= new StartNode();

    public override void Delete(bool force)
    {
        if(force) base.Delete(true);
    }
    
    protected void OnDestroy()
    {
        FlowChartManager.Branches.Remove(_node.BranchID);
    }
}