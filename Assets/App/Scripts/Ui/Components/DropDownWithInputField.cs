using System;
using System.Collections.Generic;
using System.Linq;
using Arcube;
using Arcube.UiManagement;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class DropDownWithInputField : MonoBehaviour
{
    [SerializeField] private TMP_InputField ip_field;
    [SerializeField] private TMP_Dropdown _dropdown;
    [SerializeField] private ButtonImage b_delete;
    private void Reset()
    {
        ip_field = GetComponentInChildren<TMP_InputField>();
        _dropdown = GetComponentInChildren<TMP_Dropdown>();
        b_delete = GetComponentInChildren<ButtonImage>();
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
    
    public string Text => ip_field.text;

    public UnityEvent<string> onValueChanged;
    private void Start()
    {
        _dropdown.onValueChanged.AddListener(value =>
        {
            if (value == 0)
            {
                ip_field.text = "";
                return;
            }

            var v = _variables[value - 1].Name;
            onValueChanged?.Invoke(v);
            ip_field.text = v;

            name = v;
        });
        
        ip_field.onSubmit.AddListener((value) =>
        {
            onValueChanged?.Invoke(value);
            name = value;
        });
    }

    //exposed variables
    private List<Variable> _variables;
    public void Set(List<Variable> variables, Variable selected)
    {
        try
        {
            _variables = variables;
            var variableNames = _variables.Select(variable => variable.Name).ToList();
            variableNames.Insert(0, "New");
            _dropdown.options = variableNames.Select(n => new TMP_Dropdown.OptionData(n)).ToList();
            gameObject.SetActive(true);

            if (selected != null)
            {
                if (variableNames.Contains(selected.Name))
                {
                    var selectedIndex = variableNames.IndexOf(selected.Name);
                    _dropdown.SetValueWithoutNotify(selectedIndex);
                }

                ip_field.SetTextWithoutNotify(selected.Name);
                name = !string.IsNullOrEmpty(selected.Name) ? selected.Name : "new";
            }

            if(!b_delete) return;
            b_delete.OnClick.AddListener(() =>
            {
                if (transform.parent.childCount == 1)
                {
                    MessageUi.Show("At least one variable is required");
                    return;
                }

                Destroy(gameObject);
            });
        }
        catch (Exception e)
        {
            Log.AddException(e);
        }
    }

    public void SetActive(bool value)
    {
        _dropdown.interactable = value;
        ip_field.interactable = value;
    }
}