using UnityEngine;

namespace Arcube
{
    public enum LogType
    {
        Default,
        Warning,
        Error,
        Highlight,
        Priority,
    }

    [CreateAssetMenu(fileName = "LogSettings", menuName = "AppData/LogSettings")]
    public class LogColorSettings : ScriptableObject
    {
        public Color defaultColor = Color.white;
        public Color warningColor = Color.yellow;
        public Color errorColor = Color.red;
        public Color highlightColor = Color.green;

        public Color GetColor(LogType logType) => logType switch
        {
            LogType.Default => defaultColor,
            LogType.Warning => warningColor,
            LogType.Error => errorColor,
            LogType.Highlight => highlightColor,
            _ => defaultColor,
        };
    }
}