using UnityEngine;

namespace Arcube.Utility
{
    public class EnvironmentController : MonoBehaviour
    {
        [SerializeField] private Environment environment;
        private static EnvironmentController Instance { get; set; }
        private void Awake() => Instance = this;
        public static Environment Env => Instance.environment;
    }
}