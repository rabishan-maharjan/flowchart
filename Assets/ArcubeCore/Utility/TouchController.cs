using UnityEngine;
using UnityEngine.Events;

namespace Arcube
{
    public enum ControllerInputType
    {
        Tap,
        Left,
        Right,
        Up,
        Down,
    }

    public abstract class Controller: MonoBehaviour
    {
        public UnityEvent<ControllerInputType> onInputStateChanged;
    }

    public class TouchController : Controller
    {
        [SerializeField] private float tapThreshold = 0.1f;
        private Vector2 initPosition;
        private float initTime;
        private void Update()
        {
            if (Input.touchCount <= 0) return;

            var touch = Input.GetTouch(0);
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    initPosition = touch.position;
                    initTime = Time.time;
                    break;
                case TouchPhase.Moved:
                    break;
                case TouchPhase.Stationary:
                    break;
                case TouchPhase.Ended:
                    var deltaTime = Time.time - initTime;
                    if (deltaTime < tapThreshold)
                    {
                        onInputStateChanged?.Invoke(ControllerInputType.Tap);
                        return;
                    }

                    var finalPosition = touch.position;
                    var deltaPosition = finalPosition - initPosition;
                    if(Mathf.Abs(deltaPosition.y) > Mathf.Abs(deltaPosition.x))
                    {
                        if(deltaPosition.y > 0) onInputStateChanged?.Invoke(ControllerInputType.Up);
                        else onInputStateChanged?.Invoke(ControllerInputType.Down);
                    }
                    else if(Mathf.Abs(deltaPosition.x) > Mathf.Abs(deltaPosition.y))
                    {
                        if(deltaPosition.x > 0) onInputStateChanged.Invoke(ControllerInputType.Right);
                        else onInputStateChanged.Invoke(ControllerInputType.Left);
                    }

                    break;
                case TouchPhase.Canceled:
                    break;
            }
        }
    }
}