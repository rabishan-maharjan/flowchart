using UnityEngine;

public class LevelIndicator : MonoBehaviour
{
    public void SetLevel(int value)
    {
        foreach (Transform t in transform)
        {
            t.gameObject.SetActive(t.GetSiblingIndex() < value);
        }
    }
}
