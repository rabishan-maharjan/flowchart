using UnityEngine;

public class LogicNodeObject : CommandObject
{
    public ConnectorObject connectorTrue;
    public ConnectorObject connectorFalse;
    public override Node Node => _node ??= new LogicCommand();
    protected override void OpenCommandUi()
    {
    }

    protected override bool CanConnect(ConnectorObject connectorObject)
    {
        if (connectorTrue)
        {
            if (connectorTrue.NextNodeObject == connectorObject.ParentNodeObject)
            {
                Debug.LogWarning("cyclical");
                connectorTrue.Clear();
                return false;
            }

            if (connectorTrue == connectorObject)
            {
                Debug.LogWarning("Self connection");
                connectorTrue.Clear();
                return true;
            }
        }
        
        if (connectorFalse)
        {
            if (connectorFalse.NextNodeObject == connectorObject.ParentNodeObject)
            {
                Debug.LogWarning("cyclical");
                connectorFalse.Clear();
                return false;
            }

            if (connectorFalse == connectorObject)
            {
                Debug.LogWarning("Self connection");
                connectorFalse.Clear();
                return true;
            }
        }
        
        return base.CanConnect(connectorObject);
    }
}