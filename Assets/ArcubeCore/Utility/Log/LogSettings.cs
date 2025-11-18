using UnityEngine;

namespace Arcube
{
    [CreateAssetMenu(fileName = "LogSettings", menuName = "AppData/LogSettings", order = 0)]
    public class LogSettings : ScriptableObject
    {
        public bool createText = true;
        public bool showNormal = true;
        public bool showTest = false;
        public bool showHighlights = true;
        public bool showWarning = true;
        public bool showError = true;
        public bool showException = true;
        public bool showPriority = true;
        public bool saveLogs = false;
    }
}