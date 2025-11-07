using Arcube.Api;
using UnityEngine;

namespace Arcube.Utility
{
    public enum EnvironmentType
    {
        Editor,
        Development,
        Production,
    }
    
    [CreateAssetMenu(fileName = "Environment", menuName = "Ludo/Environment", order = 0)]
    public class Environment : ScriptableObject
    {
        public EnvironmentType type;
        public Urls urls;
        public LogSettings logSettings;
    }
}