using Arcube.UiManagement;
using UnityEngine.EventSystems;

public class GraphObject : PanelItem, IPointerClickHandler
{
    protected GraphPanelUi GraphPanelUi;
    protected override void Awake()
    {
        base.Awake();
        GraphPanelUi = GetComponentInParent<GraphPanelUi>();
    }
    
    public virtual void OnPointerClick(PointerEventData eventData)
    {
        Select();
    }

    protected virtual void Select()
    {
        GraphPanelUi.Selected = this;
    }

    public virtual void Delete(bool force)
    {
        Destroy(gameObject);
    }
}