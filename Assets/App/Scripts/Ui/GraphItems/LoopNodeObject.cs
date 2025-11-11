using System.Threading.Tasks;
using Arcube;
using UnityEngine;

public class LoopNodeObject : CommandObject
{
    [field: SerializeField] public ConnectorObject ConnectorLoopObject { get; set; }
    public override Node Node => _node ??= new LoopCommand();

    protected override async Task OpenCommandUi()
    {
        await UiManager.GetUi<LoopCommandUi>().Open((LoopCommand)Node);
        await base.OpenCommandUi();
    }

    protected override bool CanConnect(ConnectorObject connectorObject)
    {
        if (!ConnectorLoopObject) return true;

        if (ConnectorLoopObject.NextNodeObject == connectorObject.ParentNodeObject)
        {
            Debug.LogWarning("cyclical");
            return false;
        }

        if (ConnectorLoopObject == connectorObject)
        {
            Debug.LogWarning("Self connection");
            return false;
        }

        return base.CanConnect(connectorObject);
    }

    public override void GenerateCode(FlowChartManager flowChartManager)
    {
        if (ConnectorLoopObject && ConnectorLoopObject.NextNodeObject)
        {
            var loopCommand = (LoopCommand)Node;
            var nextNode = ConnectorLoopObject.NextNodeObject.Node;
            if (nextNode != null)
            {
                loopCommand.NodeLoop = nextNode.ID;
            }    
        }

        base.GenerateCode(flowChartManager);
    }
}