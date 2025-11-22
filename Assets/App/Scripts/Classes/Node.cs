using System;
using Arcube;

public class Node
{
    public string ID;
    public string BranchID;
    public Vector2Simple AnchoredPosition;
    public string Name;
    public string NextNode;
    protected Node()
    {
        ID = Guid.NewGuid().ToString();
    }

    public virtual bool IsVariableUsed(string variable) => false;
}

public class StartNode : Node
{
    public StartNode()
    {
        BranchID = ID;
        Name = "StartNode";
        AppManager.GetManager<FlowChartManager>().Branches.Add(BranchID);
    }
}

public class EndNode : Node
{
    public EndNode()
    {
        Name = "EndNode";
    }
}