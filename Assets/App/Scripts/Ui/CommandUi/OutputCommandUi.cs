using System.Collections.Generic;
using System.Linq;
using Arcube;
using Arcube.UiManagement;
using UnityEngine;

public class OutputCommandUi : CommandUi
{
    [SerializeField] private VariableSelector variableSelectorPrefab;
    [SerializeField] private ButtonImage b_add;
    protected override void Reset()
    {
        transform.TryFindObject(nameof(b_add), out b_add);
        transform.TryFindObject(nameof(variableSelectorPrefab), out variableSelectorPrefab);

        base.Reset();
    }

    private List<Variable> _allVariables;
    protected override void SetUi()
    {
        _allVariables = AppManager.GetManager<FlowChartManager>().ActiveVariables;
        if (_allVariables.Count == 0)
        {
            //in future just show text field
        }
        
        b_add.OnClick.AddListener(() =>
        {
            var variableSelector = Instantiate(variableSelectorPrefab, variableSelectorPrefab.transform.parent);
            var variablesNames = _allVariables.Select(v => v.Name).ToList();
            variablesNames.Insert(0, "Select");
            variablesNames.Insert(1, "New");
            variableSelector.Set(variablesNames);
            
            b_add.transform.SetAsLastSibling();
        });
    }

    protected override void Apply()
    {
        //update graph
        var outputCommand = (OutputCommand)Command;
        foreach (var variableSelector in GetComponentsInChildren<VariableSelector>())
        {
            var variable = variableSelector.GeVariable();
            if (variable != null) outputCommand.Variables.Add(variable);
        }

        Close();
    }
}