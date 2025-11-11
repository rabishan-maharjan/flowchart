using System.Collections.Generic;
using System.Linq;
using Arcube;
using TMPro;
using UnityEngine;

public class OperationCommandUi : CommandUi
{
    [SerializeField] private TMP_Dropdown dr_variable;
    
    [SerializeField] private DropDownWithInputField v_1;
    [SerializeField] private DropDownWithInputField v_2;
    [SerializeField] private TMP_InputField ip_operator;
    protected override void Reset()
    {
        transform.TryFindObject(nameof(dr_variable), out dr_variable);
        transform.TryFindObject(nameof(v_1), out v_1);
        transform.TryFindObject(nameof(v_2), out v_2);
        transform.TryFindObject(nameof(ip_operator), out ip_operator);
        
        base.Reset();
    }

    private List<Variable> _allVariables;
    private FlowChartManager _flowChartManager;
    protected override void SetUi()
    {
        _flowChartManager = AppManager.GetManager<FlowChartManager>();
        _allVariables = _flowChartManager.ActiveVariables;
        var variableNames = _allVariables.Where(v => v.Exposed).Select(variable => variable.Name).ToList();
        dr_variable.options = variableNames.Select(n => new TMP_Dropdown.OptionData(n)).ToList();
        
        v_1.Set(_allVariables);
        v_2.Set(_allVariables);
        
        base.SetUi();
    }

    protected override void Apply()
    {
        if (_allVariables.Count == 0)
        {
            Debug.Log("Not enough variables");
            return;
        }
        
        var operationCommand = (OperationCommand)Command;

        var v = _allVariables[dr_variable.value];
        operationCommand.Assignee = v.ID;
        v_1.Value.Type = v.Type;
        v_2.Value.Type = v.Type;
        operationCommand.Variable1 = v_1.Value.ID;
        operationCommand.Variable2 = v_2.Value.ID;
        operationCommand.Operator = ip_operator.text;
        
        base.Apply();
    }
}