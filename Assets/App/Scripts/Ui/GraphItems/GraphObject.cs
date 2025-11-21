using Arcube.UiManagement;
using UnityEngine.EventSystems;

public class GraphObject : PanelItem, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    protected bool Editable = true;
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