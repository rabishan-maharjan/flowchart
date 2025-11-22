using Arcube.UiManagement;
using UnityEngine.EventSystems;

public class GraphObject : PanelItem, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    protected override void Awake()
    {
        GraphPanelUi.IsDirty = true;
        base.Awake();
    }

    private void OnDestroy()
    {
        GraphPanelUi.IsDirty = true;
    }

    public bool Editable { get; protected set; } = true;
    public virtual void OnPointerClick(PointerEventData eventData)
    {
        if(!Editable) return;
        
        Select();
    }

    protected virtual void Select()
    {
        GraphPanelUi.Selected = this;
    }

    public virtual void Delete(bool force)
    {
        if(!Editable) return;
        
        Destroy(gameObject);
    }

    protected bool Hovered;
    public void OnPointerEnter(PointerEventData eventData)
    {
        Hovered = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Hovered = false;
    }
}