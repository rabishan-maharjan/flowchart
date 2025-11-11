using System.Collections.Generic;
using System.Linq;
using Arcube;
using Arcube.UiManagement;
using TMPro;
using UnityEngine;

public class VariableSelector : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown dr_add_variable;
    [SerializeField] private FlowInputField ip_text;
    [SerializeField] private ButtonImage b_delete;

    private void Reset()
    {
        transform.TryFindObject(nameof(dr_add_variable), out dr_add_variable);
        transform.TryFindObject(nameof(ip_text), out ip_text);
        transform.TryFindObject(nameof(b_delete), out b_delete);
    }

    private List<Variable> _allVariables;
    public void Set(List<string> choices)
    {
        _allVariables = AppManager.GetManager<FlowChartManager>().ActiveVariables;
        dr_add_variable.options = choices.Select(n => new TMP_Dropdown.OptionData(n)).ToList();
        
        dr_add_variable.onValueChanged.AddListener((value) =>
        {
            if (choices[value] == "New")
            {
                ip_text.gameObject.SetActive(true);
                dr_add_variable.SetValueWithoutNotify(0);
                dr_add_variable.gameObject.SetActive(false);
            }
            else
            {
                dr_add_variable.gameObject.SetActive(true);    
            }
        });
        
        ip_text.OnDelete += () => { dr_add_variable.gameObject.SetActive(true); };
        
        b_delete.OnClick.AddListener(() =>
        {
            Destroy(gameObject);
        });
        
        gameObject.SetActive(true);
    }
    
    public string GeVariable()
    {
        if (string.IsNullOrWhiteSpace(ip_text.Text))
            return dr_add_variable.value == 0 ? null : _allVariables[dr_add_variable.value - 2].ID;
        
        var v = new Variable()
        {
            Type = VariableType.String,
            Name = ip_text.Text,
            Value = ip_text.Text,
            Assigned = true
        };
            
        AppManager.GetManager<FlowChartManager>().AddVariable(v);
        return v.ID;
    }
}