public class EndNodeObject : NodeObject
{
    public override Node Node => _node ??= new EndNode();
    public override void Delete(bool force)
    {
        if(force) base.Delete(true);
    }
}