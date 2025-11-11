using System.Threading.Tasks;
using Arcube;
using UnityEngine;

public class LogicNodeObject : CommandObject
{
    public ConnectorObject connectorTrue;
    public ConnectorObject connectorFalse;
    public override Node Node => _node ??= new LogicCommand();
    protected override async Task OpenCommandUi()
    {
        await UiManager.GetUi<LogicCommandUi>().Open((LogicCommand)Node);
        await base.OpenCommandUi();
    }

    protected override bool CanConnect(ConnectorObject connectorObject)
    {
        if (connectorTrue)
        {
            if (connectorTrue.NextNodeObject == connectorObject.ParentNodeObject)
            {
                Debug.LogWarning("cyclical");
                return false;
            }

            if (connectorTrue == connectorObject)
            {
                Debug.LogWarning("Self connection");
                return false;
            }
        }
        
        if (connectorFalse)
        {
            if (connectorFalse.NextNodeObject == connectorObject.ParentNodeObject)
            {
                Debug.LogWarning("cyclical");
                return false;
            }

            if (connectorFalse == connectorObject)
            {
                Debug.LogWarning("Self connection");
                return false;
            }
        }
        
        return base.CanConnect(connectorObject);
    }

    public override void GenerateCode(FlowChartManager flowChartManager)
    {
        if (connectorFalse && connectorFalse.NextNodeObject)
        {
            var loopCommand = (LogicCommand)Node;
            var nextNode = connectorFalse.NextNodeObject.Node;
            if (nextNode != null)
            {
                loopCommand.NodeFalse = nextNode.ID;
            }    
        }
        
        if (connectorTrue && connectorTrue.NextNodeObject)
        {
            var loopCommand = (LogicCommand)Node;
            var nextNode = connectorTrue.NextNodeObject.Node;
            if (nextNode != null)
            {
                loopCommand.NodeTrue = nextNode.ID;
            }    
        }
        
        base.GenerateCode(flowChartManager);
    }
}