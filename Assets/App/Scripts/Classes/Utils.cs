using UnityEngine;

public static class FlowChartUtils
{
    public static void PositionUIAtMousePosition(Canvas canvas, RectTransform uiElement)
    {
        // Get the current mouse position in screen space
        Vector2 mousePosition = Input.mousePosition;

        // Convert the screen space position to local position relative to the canvas
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, mousePosition, canvas.worldCamera, out Vector2 localPoint);

        // Get the size of the UI element
        var size = uiElement.sizeDelta;

        // Get the size of the canvas
        var canvasRect = canvas.GetComponent<RectTransform>();
        var canvasSize = canvasRect.sizeDelta;

        // Calculate the position of the UI element relative to the cursor
        Vector2 position = localPoint;

        // Check if the UI element can fit to the right of the cursor
        if (localPoint.x + size.x <= canvasSize.x / 2)
        {
            position.x += size.x / 2; // Place the UI element to the right of the cursor
        }
        // Otherwise, place it to the left of the cursor
        else if (localPoint.x - size.x >= -canvasSize.x / 2)
        {
            position.x -= size.x / 2; // Place the UI element to the left of the cursor
        }

        // Check if the UI element can fit above the cursor
        if (localPoint.y + size.y <= canvasSize.y / 2)
        {
            position.y += size.y / 2; // Place the UI element above the cursor
        }
        // Otherwise, place it below the cursor
        else if (localPoint.y - size.y >= -canvasSize.y / 2)
        {
            position.y -= size.y / 2; // Place the UI element below the cursor
        }

        // Set the position of the UI element
        uiElement.anchoredPosition = position;
    }
}
