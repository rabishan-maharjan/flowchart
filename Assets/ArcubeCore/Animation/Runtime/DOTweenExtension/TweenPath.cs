using UnityEngine;

public class TweenPath : MonoBehaviour {
    // Start is called before the first frame update
    public Vector3 position;
    public Vector3 rotation;
    private void Reset() {
        position = transform.position;
        rotation = transform.rotation.eulerAngles;
    }

    public void SetTransform() {
        transform.position = position;
        Quaternion rot = transform.rotation;
        rot.eulerAngles = rotation;
        transform.rotation = rot;
    }
}