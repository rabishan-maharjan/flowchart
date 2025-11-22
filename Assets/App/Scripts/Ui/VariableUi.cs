using System;
using System.Linq;
using System.Threading.Tasks;
using Arcube;
using Arcube.UiManagement;
using TMPro;
using UnityEngine;

public enum VariableType
{
    Dynamic,
    String,
    Bool,
    Number,
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
            
            tb_value.gameObject.SetActive(type == VariableType.Bool);
            ip_value.gameObject.SetActive(type != VariableType.Bool);
            
            switch (type)
            {
                case VariableType.Bool:
                    tb_value.IsOn = false;
                    break;
                case VariableType.Number:
                    ip_value.Text = "0";
                    ip_value.ContentType = TMP_InputField.ContentType.DecimalNumber;
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
        var index = dr_type.options.FindIndex(option => option.text == variable.Type.ToString());
        dr_type.value = index >= 0 ? index : 0;
        dr_type.RefreshShownValue();
            
        ip_name.Text = variable.Name;
        var isBoolean = variable.Type == VariableType.Bool;
        
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

        _variable.Name = ip_name.Text;
        
        var flowchartManager = AppManager.GetManager<FlowChartManager>();
        if (flowchartManager.ActiveVariables.FirstOrDefault(v => v.Name == ip_name.Text && v.ID != _variable.ID) != null)
        {
            MessageUi.Show($"Name {_variable.Name} is already used");
            return;
        }
        
        _variable.Type = (VariableType)dr_type.value;
        if (_variable.Type == VariableType.Dynamic)
        {
            _variable.Type = Variable.DetectType(ip_value.Text);
            if (_variable.Type == VariableType.Bool)
            {
                tb_value.IsOn = ip_value.Text == "true";
            }
        }
        
        if(_variable.Type != VariableType.Bool) _variable.Value = ip_value.Text;
        else _variable.Value = tb_value.IsOn ? "true" : "false";
        
        _variable.Assigned = true;
        _variable.Scope = VariableScope.Functional;
        
        flowchartManager.AddVariable(_variable);
        
        Close();
    }
}