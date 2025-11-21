using System.Collections;
using System.Threading.Tasks;
using Arcube;
using UnityEngine;

public class ForLoopNodeObject : CommandObject
{
    [field: SerializeField] public ConnectorObject ConnectorLoopObject { get; set; }
    public override Node Node => _node ??= new ForLoopCommand();

    protected override async Task OpenCommandUi()
    {
        await UiManager.GetUi<ForLoopCommandUi>().Open((ForLoopCommand)Node);
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
            var loopCommand = (ForLoopCommand)Node;
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
        var loopCommand =(ForLoopCommand)Node; 
        loopCommand.OnLoopStep += () =>
        {
            t_compile.text = loopCommand.GetValueDescription();
        };
    }
    
    public override void MoveBranchNodes(Vector2 localPoint)
    {
        var next = ConnectorLoopObject.NextNodeObject;
        while (next)
        {
            next.Move(localPoint);
            next.MoveBranchNodes(localPoint);
            if (!next.ConnectorObject) break;
            next = next.ConnectorObject.NextNodeObject;
        }
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
        
        var bt = connector.transform;
        var prevPosition = ConnectorObject.RectTransform.anchoredPosition;
        ConnectorObject.transform.position = new Vector3(transform.position.x, bt.transform.position.y - 50);
        var delta = ConnectorObject.RectTransform.anchoredPosition - prevPosition;
        if(delta.y != 0) MoveAllFollowingNodes(new Vector2(0, delta.y));
    }
}