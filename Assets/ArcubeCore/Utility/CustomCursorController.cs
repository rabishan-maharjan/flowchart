// 11/20/2025 AI-Tag
// This was created with the help of Assistant, a Unity Artificial Intelligence product.

using UnityEngine;

public class CustomCursorController : MonoBehaviour
{
    private RectTransform _cursorRectTransform;
    private void Start()
    {
        // Get the RectTransform of the UI Image
        _cursorRectTransform = GetComponent<RectTransform>();

        // Hide the default system cursor
        Cursor.visible = false;
    }

    private void Update()
    {
        // Update the position of the custom cursor to follow the mouse
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _cursorRectTransform.parent as RectTransform,
            Input.mousePosition,
            null,
            out var cursorPosition
        );

        _cursorRectTransform.localPosition = cursorPosition;
    }
}