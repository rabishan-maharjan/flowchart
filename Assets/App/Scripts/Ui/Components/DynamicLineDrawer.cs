using System.Threading.Tasks;
using UnityEngine;

public class DynamicLineDrawer : MonoBehaviour
{
    private RectTransform _startNodeConnector; // Reference to StartNode->Connector
    private RectTransform _endNode; // Reference to EndNode
    private LineRenderer _lineRenderer; // Single LineRenderer
    private GameObject _arrow; // Arrow instance
    private Vector3 _previousStartPosition; // Previous position of StartNode->Connector
    private Vector3 _previousEndPosition; // Previous position of EndNode
    private GraphSettings _graphSettings;
    private bool _set;
    [SerializeField] private bool drawArrow = true;

    public async Task Set(RectTransform endNode, bool shouldDrawArrow)
    {
        drawArrow = shouldDrawArrow;
        await Set(endNode);
    }
    
    public async Task Set(RectTransform endNode)
    {
        _set = true;
        _endNode = endNode;
        _startNodeConnector = (RectTransform)transform.GetChild(0);
        
        _graphSettings = GraphSettings.Instance;
        // Add a LineRenderer component to the GameObject
        if (!_lineRenderer)
        {
            _lineRenderer = gameObject.AddComponent<LineRenderer>();
            _lineRenderer.material = _graphSettings.lineMaterial;
            _lineRenderer.startWidth = _graphSettings.lineWidth;
            _lineRenderer.endWidth = _graphSettings.lineWidth;
            _lineRenderer.useWorldSpace = true;
            if(drawArrow) _arrow = Instantiate(_graphSettings.arrowPrefab, transform);
        }

        // Initialize previous positions
        _previousStartPosition = _startNodeConnector.position;
        _previousEndPosition = _endNode.position;

        await Task.Yield();
        // Ensure the line and arrow are updated at the start
        UpdateLineAndArrow();
    }

    private void Update()
    {
        if(!_set) return;

        if (!_startNodeConnector || !_endNode || !_lineRenderer)
        {
            Destroy(this);
            return;
        }
        
        // Update the line and arrow only if positions have changed
        if (_startNodeConnector.position == _previousStartPosition && _endNode.position == _previousEndPosition) return;
        UpdateLineAndArrow();
        _previousStartPosition = _startNodeConnector.position;
        _previousEndPosition = _endNode.position;
    }

    private void UpdateLineAndArrow()
    {
        if (!_startNodeConnector || !_endNode || !_lineRenderer) return;

        // Get positions in world space
        var startPosition = _startNodeConnector.position;
        var endPosition = _endNode.position;

        // Calculate the slope
        var direction = endPosition - startPosition;
        var slope = Mathf.Abs(direction.x) < 0.0001f ? float.PositiveInfinity : // vertical line
            Mathf.Abs(direction.y / direction.x);

        if (Mathf.Approximately(slope, 0)) // Check if slope is exactly 0 (horizontal line)
        {
            // Draw a single straight horizontal line
            _lineRenderer.positionCount = 2;
            _lineRenderer.SetPosition(0, startPosition);
            _lineRenderer.SetPosition(1, endPosition);
        }
        else if (slope < Mathf.Tan(Mathf.Deg2Rad * _graphSettings.slopeThreshold)) // Check if slope is less than the threshold
        {
            // Calculate intermediate point for perpendicular line
            var midPoint = new Vector3(endPosition.x, startPosition.y, startPosition.z);

            // Set positions for the two segments
            _lineRenderer.positionCount = 3;
            _lineRenderer.SetPosition(0, startPosition);
            _lineRenderer.SetPosition(1, midPoint);
            _lineRenderer.SetPosition(2, endPosition);
        }
        else
        {
            // Draw a single straight line
            _lineRenderer.positionCount = 2;
            _lineRenderer.SetPosition(0, startPosition);
            _lineRenderer.SetPosition(1, endPosition);
        }

        // Update arrow position and rotation based on the last segment of the line
        if(drawArrow) UpdateArrow();
    }

    private void UpdateArrow()
    {
        if (_lineRenderer.positionCount < 2) return;

        // Get the last two positions of the line
        var startPosition = _lineRenderer.GetPosition(_lineRenderer.positionCount - 2);
        var endPosition = _lineRenderer.GetPosition(_lineRenderer.positionCount - 1);

        // Convert world space position to local canvas space
        var localEndPosition = transform.InverseTransformPoint(endPosition);

        // Set the arrow position to the end of the last line segment in local canvas space
        _arrow.transform.localPosition = localEndPosition;

        // Calculate the direction of the last line segment
        var direction = endPosition - startPosition;

        // Calculate the rotation based on the direction of the last line segment
        var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        _arrow.transform.localRotation = Quaternion.Euler(0, 0, angle - _startNodeConnector.transform.localEulerAngles.z);
    }

    private void OnDestroy()
    {
        Destroy(_lineRenderer);
        if(_arrow) Destroy(_arrow.gameObject);
        
        Debug.LogWarning($"Destroying line drawer {name} {Time.time}", gameObject);
    }
}