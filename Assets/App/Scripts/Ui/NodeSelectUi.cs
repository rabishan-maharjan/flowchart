using Arcube;
using Arcube.UiManagement;
using UnityEngine;

public class NodeSelectUi : Ui
{
    [SerializeField] private RectTransform container;
    protected override void Reset()
    {
        container = (RectTransform)transform.GetChild(0);
        closable = true;
        deactivateOnClose = true;
        base.Reset();
    }
    
    private void Start()
    {
        gameObject.FindObject<ButtonImage>("b_close").OnClick.AddListener(Close);
    }

    protected override void SetUi()
    {
        Selected = null;
        FlowChartUtils.PositionUIAtMousePosition(canvas, container);
    }

    [SerializeField] private GraphPanelUi graphPanelUi;
    public NodeObject Selected { get; private set; }
    public void SelectGraphObject(NodeObject nodeObject)
    {
        Selected = nodeObject;
        Close();
    }
}