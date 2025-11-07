using Arcube;
using Arcube.UiManagement;
using UnityEngine;

public class GraphItemPanel : PanelItem
{
    [SerializeField] private NodeObject nodeObject;
    private void Start()
    {
        OnClick.AddListener(() =>
        {
            UiManager.GetUi<NodeSelectUi>().SelectGraphObject(nodeObject);
        });
    }
}