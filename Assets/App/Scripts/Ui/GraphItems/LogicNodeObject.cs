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
    
    [SerializeField] private DynamicLineDrawer pivot;
    private void Start()
    {
        _= pivot.Set((RectTransform)ConnectorObject.transform);
    }
    private void Update()
    {
        var connector1 = connectorTrue;
        while (connector1)
        {
            if (connector1.NextNodeObject) connector1 = connector1.NextNodeObject.ConnectorObject;
            else break;
        }
        
        var connector2 = connectorFalse;
        while (connector2)
        {
            if (connector2.NextNodeObject) connector2 = connector2.NextNodeObject.ConnectorObject;
            else break;
        }

        var selected = connector2;
        if(connector1.transform.position.y < connector2.transform.position.y)
        {
            selected = connector1;
        }

        if(!selected || selected == connectorTrue || selected == connectorFalse) return;
        
        var brt = selected.transform;
        ConnectorObject.transform.position = new Vector3(transform.position.x, brt.transform.position.y - 50);
    }
}