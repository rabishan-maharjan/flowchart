using UnityEngine;

public interface ILayout
{
    void UpdateLayout();
}

public class SimpleLayout : MonoBehaviour, ILayout
{
    [SerializeField] protected float percentWidth;
    [SerializeField] protected float percentHeight;
    private void Start()
    {
        UpdateLayout();
    }

    [ContextMenu("UpdateLayout")]
    public void UpdateLayout()
    {
        var parent = transform.parent as RectTransform;
        var parentRect = parent.rect;
        var rt = transform as RectTransform;
        var x = percentWidth > 0 ? parentRect.width * percentWidth : rt.sizeDelta.x;
        var y = percentHeight > 0 ? parentRect.height * percentHeight : rt.sizeDelta.y;
        var size = new Vector2(x, y);
        rt.sizeDelta = size;
    }
}