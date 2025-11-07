using Arcube.UiManagement;
using UnityEngine;

[RequireComponent(typeof(ButtonImage))]
public class FlowArrowObject : GraphObject
{
    private ConnectorObject Object1 { get; set; }
    private NodeObject Object2 { get; set; }
    public Transform Arrow { get; set; }
    private void Update()
    {
        if (!Input.GetKeyDown(KeyCode.Delete)) return;
        if (GraphPanelUi.Selected != this) return;
        GraphPanelUi.Selected = null;
        Delete(true);
    }
    
    public void SetConnection(ConnectorObject connectorObject, NodeObject nodeObject)
    {
        Object1 = connectorObject;
        Object2 = nodeObject;
    }

    public override void Delete(bool force)
    {
        //clear connections
        Object1.Connection = null;
        Object2.OtherConnection = null;
        Destroy(Arrow.gameObject);
        
        base.Delete(force);
    }
}