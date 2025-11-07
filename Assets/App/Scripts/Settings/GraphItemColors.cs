using UnityEngine;

[CreateAssetMenu(fileName = "GraphItemColors", menuName = "Flow/GraphItemColors")]
public class GraphItemColors : ScriptableObject
{
    public Color begin;
    public Color end;
    public Color operation;
    public Color condition;
    public Color input;
    public Color output;
}