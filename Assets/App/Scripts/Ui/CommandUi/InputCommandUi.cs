using System;
using System.Collections.Generic;
using System.Linq;
using Arcube;
using TMPro;
using UnityEngine;

public class InputCommandUi : CommandUi
{
    [SerializeField] private TMP_Dropdown dr_add_variable;
    protected override void Reset()
    {
        transform.TryFindObject(nameof(dr_add_variable), out dr_add_variable);
        
        base.Reset();
    }

    private List<Variable> _exposedVariables;
    protected override void SetUi()
    {
        try
        {
            Debug.Log("Setting ui");
            var flowChartManager = AppManager.GetManager<FlowChartManager>();
            _exposedVariables = flowChartManager.ActiveVariables.Where(v => v.Exposed).ToList();
            var variablesNames = _exposedVariables.Select(v => v.Name).ToList();
            dr_add_variable.options = variablesNames.Select(n => new TMP_Dropdown.OptionData(n)).ToList();

            var inputCommand = (InputCommand)Command;
            if (!string.IsNullOrEmpty(inputCommand.Variable))
            {
                var variable = flowChartManager.VariableMap[inputCommand.Variable];
                dr_add_variable.value = _exposedVariables.IndexOf(variable);
            }
        }
        catch (Exception e)
        {
            Log.AddException(e);
        }
    }

    protected override void Apply()
    {
        //update graph
        var inputCommand = (InputCommand)Command;
        if (_exposedVariables.Count == 0)
        {
            Debug.Log("No variables");
            return;
        }
        
        var variable = _exposedVariables[dr_add_variable.value];
        inputCommand.Variable = variable.ID;
        
        base.Apply();
    }
}