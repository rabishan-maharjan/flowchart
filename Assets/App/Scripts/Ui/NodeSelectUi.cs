using System.Threading.Tasks;
using Arcube;
using Arcube.UiManagement;
using UnityEngine;
using UnityEngine.Serialization;

public class NodeSelectUi : Ui
{
    protected override void Reset()
    {
        closable = true;
        deactivateOnClose = true;
        base.Reset();
    }

    private void Start()
    {
        gameObject.FindObject<ButtonImage>("b_close").OnClick.AddListener(Close);
    }

    [FormerlySerializedAs("graphPanel")] [SerializeField] private GraphPanelUi graphPanelUi;
    private NodeObject _selected;
    public void SelectGraphObject(NodeObject nodeObject)
    {
        _selected = nodeObject;
        Close();
    }

    public async Task<NodeObject> SelectNode()
    {
        await OpenAsync();
        return _selected;
    }
}