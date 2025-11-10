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
    
    private List<Variable> _allVariables;
    protected override void SetUi()
    {
        _allVariables = AppManager.GetManager<FlowChartManager>().ActiveVariables;
        var variablesNames = _allVariables.Select(v => v.Name).ToList();
        variablesNames.Insert(0, "Select");
        dr_add_variable.options = variablesNames.Select(n => new TMP_Dropdown.OptionData(n)).ToList();
    }

    protected override void Apply()
    {
        //update graph
        var inputCommand = (InputCommand)Command;
        if (dr_add_variable.value == 0)
        {
            Debug.Log("Select variable");
            return;
        }
        
        var variable = _allVariables[dr_add_variable.value - 1];
        inputCommand.Variable = variable;
        
        Close();
    }
}