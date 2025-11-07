using UnityEngine;

public partial class GraphPanelUi
{
    private FlowArrowObject DrawLine(ConnectorObject connectorObject, NodeObject nodeObject)
    {
        // Draw a straight line
        var line = Instantiate(linePrefab, _canvas.transform).GetComponent<FlowArrowObject>();
        var arrow = Instantiate(arrowPrefab, _canvas.transform);
        line.Arrow = arrow.transform;
        
        UpdateLine(line, connectorObject, nodeObject);
        
        return line;
    }

    public void UpdateLine(FlowArrowObject line, ConnectorObject connectorObject, NodeObject nodeObject)
    {
        var  pointA = (RectTransform)connectorObject.transform;
        var pointB = (RectTransform)nodeObject.transform;
        
        // Convert world positions to local positions relative to the canvas
        var start = _canvas.transform.InverseTransformPoint(pointA.position);
        var end = _canvas.transform.InverseTransformPoint(pointB.position);
        
        var rectTransform = line.GetComponent<RectTransform>();

        // Calculate the midpoint and set the anchored position of the line
        var midPoint = (start + end) / 2f;
        rectTransform.anchoredPosition = midPoint;

        // Set the size and rotation of the line
        var direction = end - start;
        var distance = direction.magnitude;
        rectTransform.sizeDelta = new Vector2(distance, rectTransform.sizeDelta.y); // Set the width of the line
        rectTransform.rotation = Quaternion.FromToRotation(Vector3.right, direction); // Rotate the line to face the direction
        
        UpdateArrow(line.Arrow, start, end);
    }

    private void UpdateArrow(Transform arrow, Vector3 startPos, Vector3 endPos)
    {
        var arrowRectTransform = arrow.GetComponent<RectTransform>();

        // Set the position of the arrow at the last point
        arrowRectTransform.anchoredPosition = endPos;

        // Calculate the direction from startPos to endPos
        var direction = (endPos - startPos).normalized;

        // Rotate the arrow to point from startPos to endPos
        var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        arrowRectTransform.rotation = Quaternion.Euler(0, 0, angle);
    }
}