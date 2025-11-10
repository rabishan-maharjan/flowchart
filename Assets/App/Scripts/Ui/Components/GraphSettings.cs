using UnityEngine;

[CreateAssetMenu(fileName = "GraphSettings", menuName = "Flow/Graph Settings")]
public class GraphSettings : ScriptableObject
{
    public float lineWidth = 0.1f; // Serialized line width
    public float slopeThreshold = 45f; // Serialized slope threshold in degrees
    public Material lineMaterial; // Material for the line
    public GameObject arrowPrefab; // Prefab for the arrow (UI Image)
    
    private static GraphSettings _instance;
    public static GraphSettings Instance
    {
        get
        {
            if (_instance) return _instance;
            _instance = Resources.Load<GraphSettings>("GraphSettings");
            if (!_instance)
            {
                Debug.LogError("GraphSettings instance not found in Resources folder. Please create one and place it in a Resources folder.");
            }
            return _instance;
        }
    }
}