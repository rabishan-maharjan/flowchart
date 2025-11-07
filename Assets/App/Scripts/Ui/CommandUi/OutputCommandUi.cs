using System.Collections.Generic;
using System.Linq;
using Arcube;
using TMPro;
using UnityEngine;

public class OutputCommandUi : CommandUi
{
    [SerializeField] private TMP_Dropdown dr_add_variable;
    protected override void Reset()
    {
        transform.TryFindObject(nameof(dr_add_variable), out dr_add_variable);
        
        base.Reset();
    }

    private List<Variable> _variables;
    protected override void Start()
    {
        _variables = AppManager.GetManager<FlowChartManager>().ActiveVariables;
        var variablesNames = _variables.Select(v => v.Name).ToList();
        variablesNames.Insert(0, "Select");
        dr_add_variable.options = variablesNames.Select(n => new TMP_Dropdown.OptionData(n)).ToList();
        
        base.Start();
    }

    protected override void Apply()
    {
        //update graph
        var outputCommand = (OutputCommand)Command;
        if (dr_add_variable.value == 0)
        {
            Debug.Log("Select variable");
            return;
        }
        
        var variable = _variables[dr_add_variable.value - 1];
        outputCommand.Variable = variable;
        
        Close();
    }
}