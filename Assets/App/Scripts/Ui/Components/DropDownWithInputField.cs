using System.Collections.Generic;
using System.Linq;
using Arcube;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class DropDownWithInputField : MonoBehaviour
{
    [SerializeField] private TMP_InputField ip_field;
    [SerializeField] private TMP_Dropdown _dropdown;
    private void Reset()
    {
        ip_field = GetComponentInChildren<TMP_InputField>();
    }

    public Variable Value
    {
        get
        {
            var flowChartManager = AppManager.GetManager<FlowChartManager>();
            var v = flowChartManager.ActiveVariables.FirstOrDefault(v => v.Name == ip_field.text);
            if (v != null) return v;
            if (string.IsNullOrEmpty(ip_field.text)) return null;
            
            v = new Variable
            {
                Name = ip_field.text,
                Value = ip_field.text
            };
            
            flowChartManager.AddVariable(v);
            return v;
        }
    }

    public UnityEvent<string> onValueChanged;
    private void Start()
    {
        _dropdown.onValueChanged.AddListener(value =>
        {
            if(value == 0) ip_field.text = "";
            ip_field.text = _allVariables[value -  1].Name;
        });
        
        ip_field.onValueChanged.AddListener((value) =>
        {
            onValueChanged?.Invoke(value);
        });
    }

    private List<Variable> _allVariables;
    public void Set(List<Variable> allVariables)
    {
        _allVariables = allVariables;
        var variableNames = _allVariables.Where(v => v.Exposed).Select(variable => variable.Name).ToList();
        variableNames.Insert(0, "New");
        _dropdown.options = variableNames.Select(n => new TMP_Dropdown.OptionData(n)).ToList(); 
        gameObject.SetActive(true);
    }
}