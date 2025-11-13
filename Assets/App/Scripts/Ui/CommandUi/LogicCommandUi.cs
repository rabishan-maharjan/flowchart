using System.Collections.Generic;
using System.Linq;
using Arcube;
using Arcube.UiManagement;
using TMPro;
using UnityEngine;

public class LogicCommandUi : CommandUi
{
    [SerializeField] private TMP_Dropdown dr_variable_1;
    [SerializeField] private TMP_Dropdown dr_operator;
    [SerializeField] private DropDownWithInputField dr_variable_2;
    protected override void Reset()
    {
        base.Reset();
        
        transform.TryFindObject(nameof(dr_variable_1), out dr_variable_1);
        transform.TryFindObject(nameof(dr_operator), out dr_operator);
        transform.TryFindObject(nameof(dr_variable_2), out dr_variable_2);
    }

    private List<Variable> _allVariables;
    private List<Variable> _exposedVariables;
    protected override void Start()
    {
        base.Start();
        
        dr_variable_1.onValueChanged.AddListener((value) =>
        {
            var selected = _allVariables[value];
            UpdateVariableLists(selected.Type);
        });
    }
    
    private void UpdateVariableLists(VariableType type)
    {
        var variables = _exposedVariables.Where(v => v.Type == type).ToList();
        dr_variable_2.Set(variables, new Variable());
    }

    protected override void SetUi()
    {
        var flowChartManager = AppManager.GetManager<FlowChartManager>(); 
        _allVariables = flowChartManager.ActiveVariables;
        _exposedVariables = _allVariables.Where(v => v.Exposed).ToList();
        
        var variablesNames = _allVariables.Where(v=> v.Exposed).Select(v => v.Name).ToList();
        dr_variable_1.options = variablesNames.Select(n => new TMP_Dropdown.OptionData(n)).ToList();
        dr_operator.options = OperatorHandler.LogicOperators.Select(n => new TMP_Dropdown.OptionData(n)).ToList();
        
        variablesNames.Insert(0, "New");
        dr_variable_2.Set(_exposedVariables, new Variable());
        
        //set existing variable value
        var logicCommand = (LogicCommand)Command;
        if (_exposedVariables.Count > 0)
        {
            var selected = string.IsNullOrEmpty(logicCommand.Variable1) ? flowChartManager.VariableMap[logicCommand.Variable1] : _exposedVariables[0];
            UpdateVariableLists(selected.Type);
        }
        
        var v1 = flowChartManager.VariableMap[logicCommand.Variable1];
        if(v1 != null) dr_variable_1.value = variablesNames.IndexOf(v1.Name);
        
        
    }

    protected override void Apply()
    {
        if (_allVariables.Count == 0)
        {
            Debug.Log("Not enough variables");
            return;
        }

        var logicCommand = (LogicCommand)Command;
        var v1 = _allVariables[dr_variable_1.value];

        var v2 = dr_variable_2.Value;
        if (v2 == null)
        {
            MessageUi.Show("Variable empty");
            return;
        }
        
        if(v2.Type == VariableType.Dynamic) v2.Type = v1.Type;
        
        if (!v2.IsValid())
        {
            MessageUi.Show("Variable doesn't match type");
            return;
        }
        
        logicCommand.Variable1 = _allVariables[dr_variable_1.value].ID;
        logicCommand.Variable2 = dr_variable_2.Value.ID;
        logicCommand.Operator = OperatorHandler.LogicOperators[dr_operator.value];
        
        base.Apply();
    }
}