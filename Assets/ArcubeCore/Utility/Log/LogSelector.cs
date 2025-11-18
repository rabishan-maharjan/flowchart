using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Arcube
{
    [RequireComponent(typeof(TMP_Dropdown))]
    public class LogSelector : MonoBehaviour
    {
        [SerializeField] List<LogSettings> logSettings;
        private void Start()
        {
            var d_logSettings = GetComponent<TMP_Dropdown>();
            d_logSettings.ClearOptions();

            if (logSettings == null || logSettings.Count == 0) return;
        
            d_logSettings.options.Add(new TMP_Dropdown.OptionData("Select Log"));

            foreach (var logSetting in logSettings)
                d_logSettings.options.Add(new TMP_Dropdown.OptionData(logSetting.name));

            d_logSettings.onValueChanged.AddListener(OnSelect);
            d_logSettings.value = 1;
        }

        private void OnSelect(int index)
        {
            if (index == 0) return;
            Log.Instance.LOGSettings = logSettings[index - 1];
        }
    }
}
