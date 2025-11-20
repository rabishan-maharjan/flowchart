using System.Collections;
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

    public override void GenerateCode()
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

        base.GenerateCode();
    }

    [SerializeField] private DynamicLineDrawer pivot;
    protected override IEnumerator Start()
    {
        yield return base.Start();
        _= pivot.Set((RectTransform)ConnectorObject.transform);
        var loopCommand =(LoopCommand)Node; 
        loopCommand.OnLoopStep += () =>
        {
            Text = loopCommand.GetValueDescription();
        };
    }
    
    private void Update()
    {
        var connector = ConnectorLoopObject;
        while (connector)
        {
            if (connector.NextNodeObject) connector = connector.NextNodeObject.ConnectorObject;
            else break;
        }

        if(!connector || connector == ConnectorLoopObject) return;
        
        var brt = connector.transform;
        ConnectorObject.transform.position = new Vector3(transform.position.x, brt.transform.position.y - 50);
    }
}