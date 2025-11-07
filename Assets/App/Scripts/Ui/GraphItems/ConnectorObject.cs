using Arcube.UiManagement;
using UnityEngine;

public class ConnectorObject : GraphObject
{
    public int id = 0;
    [field: SerializeField] public NodeObject ParentNodeObject { get; set; }
    public Vector2 offset;
    public GraphObject Connection { get; set; }
    public FlowArrowObject Line { get; set; }
    protected override void Reset()
    {
        ParentNodeObject = GetComponentInParent<NodeObject>();
        offset = new Vector2(0, 35);
        base.Reset();
    }

    private void Start()
    {
        GetComponent<ButtonImage>().OnClick.AddListener(Select);
    }

    public void Clear()
    {
        Line?.Delete(true);
    }
}