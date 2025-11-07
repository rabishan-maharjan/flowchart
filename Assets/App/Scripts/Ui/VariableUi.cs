using System;
using System.Linq;
using System.Threading.Tasks;
using Arcube;
using Arcube.UiManagement;
using TMPro;
using UnityEngine;

public enum VariableType
{
    String,
    Boolean,
    Int,
    Float,
}

public class VariableUi : Ui
{
    [SerializeField] private TMP_Dropdown dr_type;
    [SerializeField] private InputFieldSimple ip_name;
    [SerializeField] private InputFieldSimple ip_value;
    [SerializeField] private ToggleButton tb_value;
    private void Start()
    {
        gameObject.FindObject<ButtonImage>("b_close").OnClick.AddListener(Close);
        gameObject.FindObject<ButtonImage>("b_ok").OnClick.AddListener(SetVariable);
        
        dr_type.options = Enum.GetNames(typeof(VariableType)).Select(n => new TMP_Dropdown.OptionData(n)).ToList();
        
        dr_type.onValueChanged.AddListener((value) =>
        {
            var type = (VariableType)value; 
            
            tb_value.gameObject.SetActive(type == VariableType.Boolean);
            ip_value.gameObject.SetActive(type != VariableType.Boolean);
            
            switch (type)
            {
                case VariableType.Boolean:
                    break;
                case VariableType.Float:
                    ip_value.ContentType = TMP_InputField.ContentType.DecimalNumber;
                    break;
                case VariableType.Int:
                    ip_value.ContentType = TMP_InputField.ContentType.IntegerNumber;
                    break;
                case VariableType.String:
                    ip_value.ContentType = TMP_InputField.ContentType.Standard;
                    break;
            }
        });
    }

    protected override void Reset()
    {
        deactivateOnClose = true;
        closable = true;
        
        transform.TryFindObject(nameof(dr_type), out dr_type);
        transform.TryFindObject(nameof(ip_name), out ip_name);
        transform.TryFindObject(nameof(ip_value), out ip_value);
        transform.TryFindObject(nameof(tb_value), out tb_value);
        
        base.Reset();
    }

    private Variable _variable;
    public async Task<Variable> Open(Variable variable)
    {
        SetFields(variable);

        _variable = variable;
        
        await base.OpenAsync();
        
        return _variable;
    }

    private void SetFields(Variable variable)
    {
        var index = dr_type.options.FindIndex(option => option.text == variable.Type);
        dr_type.value = index >= 0 ? index : 0;
        dr_type.RefreshShownValue();
            
        ip_name.Text = variable.Name;
        var isBoolean = variable.Type == "Boolean";
        
        tb_value.gameObject.SetActive(isBoolean);
        ip_value.gameObject.SetActive(!isBoolean);
        
        if (isBoolean) tb_value.IsOn = variable.Value == "true";
        else ip_value.Text = variable.Value;
    }
    
    private void SetVariable()
    {
        if (string.IsNullOrWhiteSpace(ip_name.Text))
        {
            MessageUi.Show("Name is required");
            return;
        }

        if (string.IsNullOrWhiteSpace(ip_value.Text))
        {
            MessageUi.Show("Value is required");
            return;
        }
        
        _variable.Name = ip_name.Text;
        _variable.Type = dr_type.options[dr_type.value].text;
        if(_variable.Type != "Boolean") _variable.Value = ip_value.Text;
        else _variable.Value = tb_value ? "true" : "false";
        
        _variable.Assigned = true;
        
        AppManager.GetManager<FlowChartManager>().AddVariable(_variable);
        
        Close();
    }
}