using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class CommandObject : NodeObject
{
    private Command Command => (Command)Node;
    
    private float _lastClickTime;
    private const float DoubleClickTime = 0.3f;

    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);
        
        if (Time.time - _lastClickTime < DoubleClickTime)
        {
            GraphPanelUi.Selected = null;
            _ = OpenCommandUi();
            _lastClickTime = 0;
        }
        else
        {
            _lastClickTime = Time.time;
        }
    }

    protected virtual async Task OpenCommandUi()
    {
        await Task.Yield();
        Text = Command.GetDescription();
    }
}