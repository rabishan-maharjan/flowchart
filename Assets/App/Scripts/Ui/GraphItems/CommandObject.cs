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
            OpenCommandUi();
            _lastClickTime = 0;
        }
        else
        {
            _lastClickTime = Time.time;
        }
    }

    protected abstract void OpenCommandUi();
}