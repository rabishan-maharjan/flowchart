using System;
using Arcube;

public class Node
{
    public string ID;
    public Vector2Simple AnchoredPosition;
    public string Name;
    public string NextNode;
    protected Node()
    {
        ID = Guid.NewGuid().ToString();
    }

    public virtual void Connect(Node node)
    {
        NextNode = node.ID;
    }
}

public class StartNode : Node
{
    public StartNode()
    {
        Name = "StartNode";
    }
}

public class EndNode : Node
{
    public EndNode()
    {
        Name = "EndNode";
    }
}